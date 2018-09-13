using AutoMapper;
using JwtAuth.Services.Data;
using JwtAuthModels.Data;
using JwtAuthModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JwtAuth.Services
{
    public class UserAuthentication : IUserAuthentication
    {
        private readonly IMapper _mapper;
        private readonly IJwtAuthentication _authService;
        private readonly IHttpContextAccessor _http;
        private readonly IConfiguration _conf;
        private readonly ILoginData _loginData;

        private readonly int SALT_SIZE = 64;
        private readonly int HASH_SIZE = 64;
        private readonly int HASH_ITERATIONS = 10000;

        public UserAuthentication(IMapper mapper, IHttpContextAccessor http,
                                  IConfiguration conf, IJwtAuthentication authService, ILoginData loginData)
        {
            _mapper = mapper;
            _authService = authService;
            _http = http;
            _conf = conf;
            _loginData = loginData;
        }

        public ApiResponse Register(UserViewModel user)
        {
            byte[] salt = GenerateSalt(SALT_SIZE);
            byte[] passwordHash = GeneratePassword(user.Password, salt);
            User newUser = _mapper.Map<User>(user);
            newUser.Salt = salt;
            newUser.Hash = passwordHash;

            if (_loginData.UserExists(newUser))
                return new ApiResponse { Error = $"User {newUser.Email} already exists." };

            _loginData.RegisterUser(newUser);
            return new ApiResponse { Data = $"User {newUser.Email} registered successfully." };
        }

        public ApiResponse Authenticate(UserViewModel modelUser)
        {
            User dbUser = _loginData.GetUser(modelUser.Email);
            if (dbUser == null)
                return new ApiResponse { Error = $"User {modelUser.Email} doesn't exist." };

            byte[] storedSalt = dbUser.Salt;
            byte[] incomingPass = GeneratePassword(modelUser.Password, storedSalt);

            // Check if both passwords match
            bool different = false;
            for (int i = 0; i < dbUser.Hash.Length; i++)
            {
                if (dbUser.Hash[i] != incomingPass[i])
                {
                    different = true;
                    break;
                }
            }

            if (different)
                return new ApiResponse { Error = "Incorrect password." };

            string token = _authService.GetToken(GetClaims(dbUser));
            string refreshToken = _authService.GetRefreshToken();
            Token generatedToken = BuildTokenEntity(dbUser.Id, refreshToken);
            _loginData.SaveRefreshToken(generatedToken);
            int refreshTokensCount = _loginData.GetUserRefreshTokens(dbUser.Id);

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

        private List<Claim> GetClaims(User user)
        {
            var customClaims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            return customClaims;
        }

        private Token BuildTokenEntity(int userId, string refreshToken)
        {
            Token newToken = new Token
            {
                UserId = userId,
                RefreshToken = refreshToken,
                GeneratedOn = DateTime.Now,
                Expiration = DateTime.Now.AddSeconds(_conf.GetValue<double>("JWT:RefreshExpireSeconds")),
                Origin = _http.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString()
            };

            return newToken;
        }
    }
}
