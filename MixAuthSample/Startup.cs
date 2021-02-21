using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MixAuthSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddOptions<FooAuthenticationOption>("Foo");
            services.AddOptions<BarAuthenticationOption>("Bar");
            var builder=services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Foo";
            })
            .AddScheme<FooAuthenticationOption, FooAuthenticationHandler>("Foo", "foo", options =>
            {
                // For example, can foward any requests that start with /api 
                // to the api scheme.
                options.ForwardDefaultSelector = ctx =>
                    ctx.Request.Path.StartsWithSegments("/api") ? "Bar" : null;
            })
            .AddScheme<BarAuthenticationOption,BarAuthenticationHandler>("Bar", "bar",options=> { }) ;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
