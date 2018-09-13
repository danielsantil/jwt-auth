using AutoMapper;
using JwtAuth.DataContext;
using JwtAuth.ExtensionMethods;
using JwtAuth.Services;
using JwtAuth.Services.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
            services.AddHttpContextAccessor();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            services.AddAutoMapper();
            services.ConfigureAuthentication(_configuration);
            services.AddScoped<ILoginData, SqlLoginData>();
            services.AddScoped<IJwtAuthentication, JwtAuthentication>();
            services.AddScoped<IUserAuthentication, UserAuthentication>();
            services.AddDbContext<JwtAuthDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("JwtAuth")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}

            // Implement exception handler

            app.UseAuthentication();
            app.UseMvc(routeBuilder => routeBuilder.MapRoute("Default", "/{controller}/{action}/{params?}"));

        }
    }
}
