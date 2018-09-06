using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TestAuth.Data;
using TestAuth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestAuth.Services.Data;
using TestAuth.ExtensionMethods;

namespace TestAuth
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
            services.ConfigureAuthentication(_configuration);
            services.AddScoped<IJwtAuthentication, JwtAuthentication>();
            services.AddScoped<ILoginData, SqlLoginData>();
            services.AddDbContext<LoginDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("TestAuth")));
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
