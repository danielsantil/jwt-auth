using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using JwtAuth.Repository;
using JwtAuthModels.Data;
using JwtAuthModels.ViewModels;

namespace JwtAuth.Services
{
    public class UserAuthentication : IUserAuthentication
    {
        private readonly IMapper _mapper;
        private readonly IJwtAuthentication _authService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Token> _tokenRepository;

        private readonly int SALT_SIZE = 64;
        private readonly int HASH_SIZE = 64;
        private readonly int HASH_ITERATIONS = 10000;
        private readonly string REFRESH_TOKEN_EXPIRED = "Refresh token not longer valid.";
        private readonly string INVALID_TOKEN = "Invalid token.";

        public UserAuthentication(IMapper mapper, IJwtAuthentication authService,
                                  IRepository<User> userRepository, IRepository<Token> tokenRepository)
        {
            _mapper = mapper;
            _authService = authService;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        public async Task<ApiResponse> Register(UserViewModel user)
        {
            byte[] salt = GenerateSalt(SALT_SIZE);
            byte[] passwordHash = GeneratePassword(user.Password, salt);
            User newUser = _mapper.Map<User>(user);
            newUser.Salt = salt;
            newUser.Hash = passwordHash;

            if (await _userRepository.Count(x => x.Email == newUser.Email) > 0)
                return new ApiResponse { Error = $"User {newUser.Email} already exists." };

            await _userRepository.Insert(newUser);

            return new ApiResponse { Data = $"User {newUser.Email} registered successfully." };
        }

        public async Task<ApiResponse> Authenticate(UserViewModel modelUser)
        {
            User storedUser = await _userRepository.Get(x => x.Email == modelUser.Email);
            if (storedUser == null)
                return new ApiResponse { Error = $"User {modelUser.Email} doesn't exist." };

            byte[] incomingPass = GeneratePassword(modelUser.Password, storedUser.Salt);

            if (PasswordsDontMatch(incomingPass, storedUser.Hash))
                return new ApiResponse { Error = "Incorrect password." };

            string token = _authService.GetToken(_authService.GetClaims(storedUser.Email));
            string refreshToken = _authService.GetRefreshToken();
            Token generatedToken = _authService.BuildTokenEntity(storedUser.Id, refreshToken);
            await _tokenRepository.Insert(generatedToken);
            int refreshTokensCount = await _tokenRepository.Count(t => t.UserId == storedUser.Id);

            ApiResponse response = new ApiResponse
            {
                Data = new
                {
                    accessToken = token,
                    refreshToken,
                    refreshTokensCount
                }
            };

            return response;
        }


        public async Task<ApiResponse> GetNewRefreshToken(TokenViewModel model)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                User storedUser = await GetUserFromToken(model.Token);
                Token storedToken = await _tokenRepository.Get(x => x.UserId == storedUser.Id 
                                                               && x.RefreshToken == model.OldRefreshToken);
                if (storedToken == null || DateTime.Now.CompareTo(storedToken.Expiration) >= 0)
                    return new ApiResponse { Error = REFRESH_TOKEN_EXPIRED };

                string newAccessToken = _authService.GetToken(_authService.GetClaims(storedUser.Email));
                string newRefreshToken = _authService.GetRefreshToken();
                Token newToken = _authService.BuildTokenEntity(storedUser.Id, newRefreshToken);
                await _tokenRepository.Delete(storedToken);
                await _tokenRepository.Insert(newToken);

                response.Data = new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                };
            }
            catch (Exception e)
            {
                response.Error = e.Message;
            }

            return response;
        }

        public async Task<ApiResponse> InvalidateRefreshTokens(TokenViewModel model)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                User storedUser = await GetUserFromToken(model.Token);
                if(storedUser == null)
                    return new ApiResponse { Error = INVALID_TOKEN };

                await _tokenRepository.Delete(x => x.UserId == storedUser.Id 
                                              && x.RefreshToken != model.OldRefreshToken);

                response.Data = true;
            }
            catch (Exception e)
            {
                response.Error = e.Message;
            }

            return response;
        }

        private bool PasswordsDontMatch(byte[] incomingPass, byte[] storedPass)
        {
            bool different = false;
            for (int i = 0; i < storedPass.Length; i++)
            {
                if (storedPass[i] != incomingPass[i])
                {
                    different = true;
                    break;
                }
            }
            return different;
        }

        private byte[] GenerateSalt(int saltSize)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[saltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }

        private byte[] GeneratePassword(string plainText, byte[] salt)
        {
            using (Rfc2898DeriveBytes hashFunction = new Rfc2898DeriveBytes(plainText, salt.Length))
            {
                // Generate hash
                hashFunction.Salt = salt;
                hashFunction.IterationCount = HASH_ITERATIONS;
                byte[] hash = hashFunction.GetBytes(HASH_SIZE);
                return hash;
            }
        }

        private async Task<User> GetUserFromToken(string token)
        {
            ClaimsPrincipal principal = _authService.GetPrincipalsFromExpired(token);
            string email = principal.Identities.FirstOrDefault().Claims.FirstOrDefault().Value;
            User storedUser = await _userRepository.Get(x => x.Email == email);

            return storedUser;
        }
    }
}
