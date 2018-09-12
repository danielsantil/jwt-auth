using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JwtAuth.DataContext;
using JwtAuth.ExtensionMethods;
using JwtAuth.Services;
using JwtAuth.Services.Data;
using AutoMapper;

namespace JwtAuth
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddMvc();
            services.AddAutoMapper();
            services.ConfigureAuthentication(_configuration);
            services.AddScoped<IJwtAuthentication, JwtAuthentication>();
            services.AddScoped<ILoginData, SqlLoginData>();
            services.AddDbContext<JwtAuthDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("JwtAuth")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
