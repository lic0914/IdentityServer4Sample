using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }



        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                //一
                .AddJwtBearer("Bearer", option =>
                {
                    option.Authority = "http://localhost:5000";
                    option.Audience = "api1";//要访问的API scope
                    option.RequireHttpsMetadata = false;
                });
            //二
            //.AddIdentityServerAuthentication(option =>
            //{
            //    option.Authority = "http://localhost:5000";
            //    option.ApiName = "api1";//要访问的API scope
            //    option.RequireHttpsMetadata = false;
            //});

            /* 获得token 必须使用x-www-form-urlencoded  / form-data格式请求 authority/connect/token*/

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
