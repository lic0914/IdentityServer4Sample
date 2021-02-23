using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtBearer.Samples;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

            var setting = new JwtSettings();
            var configurationSection = Configuration.GetSection(JwtSettings.Key);
            configurationSection.Bind(setting);
            services.Configure<JwtSettings>(configurationSection);

            var builder=services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(option =>
                {
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    option.SlidingExpiration = false;
                    option.LoginPath = "/Identity/Login";
                    option.ForwardDefaultSelector = ctx =>
                        ctx.Request.Path.StartsWithSegments("/api") ? JwtBearerDefaults.AuthenticationScheme : null;
                })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        //NameClaimType和RoleClaimType需与Token中的ClaimType一致
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,

                        //用于与TokenClaims中的Issuer和Audience进行对比，不一致则验证
                        ValidIssuer = "http://localhost:5000",
                        ValidAudience = "api",

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(setting.Secret))

                        /***********************************TokenValidationParameters的参数默认值***********************************/
                        // RequireSignedTokens = true,
                        // SaveSigninToken = false,
                        // ValidateActor = false,
                        // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                        // ValidateAudience = true,
                        // ValidateIssuer = true, 
                        // ValidateIssuerSigningKey = false,
                        // 是否要求Token的Claims中必须包含Expires
                        // RequireExpirationTime = true,
                        // 允许的服务器时间偏移量
                        // ClockSkew = TimeSpan.FromSeconds(300),
                        // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                        // ValidateLifetime = true
                    };
                })
            .AddScheme<FooAuthenticationOption, FooAuthenticationHandler>("Foo", "foo", options =>
            {
                // For example, can foward any requests that start with /api 
                // to the api scheme.
                // 源码 https://source.dot.net/#Microsoft.AspNetCore.Authentication/AuthenticationHandler.cs,f7092fd99fe4c4a1,references
                // ResolveTarget(scheme)
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
                endpoints.MapRazorPages();
            });
        }
    }
}
