using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthCookie.Sample
{
    class Program
    {
        private static Dictionary<string, string> _accounts;

        static Program()
        {
            _accounts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _accounts.Add("Foo", "1");
            _accounts.Add("Bar", "1");
            _accounts.Add("Baz", "1");
        }
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logger => logger
                    .AddConsole().SetMinimumLevel(LogLevel.Debug))
                .ConfigureWebHostDefaults(builder => builder
                    .ConfigureServices(svcs => svcs
                        .AddRouting()
                        .AddAuthentication(options =>
                        {
                            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        })
                        .AddCookie())
                    .Configure(app=>app
                        .UseAuthentication()
                        .UseRouting()
                        .UseEndpoints(endpoints =>
                        {
                            endpoints.Map(pattern: "/", RenderHomePageAsync);
                            endpoints.Map("Account/Login", SignInAsync);
                            endpoints.Map("Account/Logout", SignOutAsync);
                        }))
                 )
                .Build()
                .Run();
        }
        public static async Task RenderHomePageAsync(HttpContext context)
        {
            if (context?.User?.Identity?.IsAuthenticated == true)
            {
                var body = $@"<h3>Welcome {context.User.Identity.Name}</h3>";
                body += FormatClaimPrincipal(context.User);
                body += "</br><a href='Account/Logout'>Sign Out</a>";
                await context.Response.WriteAsync(@"<html><head><meta charset='utf-8'><title>Index</title></head> <body>" + body + "</body></html>");
            }
            else
            {
                await context.ChallengeAsync();
            }
        }


        public static async Task SignInAsync(HttpContext context)
        {


            if (String.CompareOrdinal(context.Request.Method, "GET") == 0)
            {
                await RenderLoginPageAsync(context, null, null, null);
            }
            else
            {
                var userName = context.Request.Form["username"];
                var password = context.Request.Form["password"];
                if (_accounts.TryGetValue(userName, out var pwd) && pwd == password)
                {
                    var identities =new List<ClaimsIdentity>
                    {
                        new ClaimsIdentity(new List<Claim>()
                        {
                            new Claim(ClaimTypes.Country, "China"),
                            new Claim(ClaimTypes.Actor, "owner")
                        },"owner"),
                        new ClaimsIdentity(new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, "lic"),
                            new Claim(ClaimTypes.Email, "11@qq.com"),
                            new Claim(ClaimTypes.Role, "admin"),
                            new Claim(ClaimTypes.Role, "sysuser")
                        },"admin"),
                    };
                   

                    //var identity = new GenericIdentity(userName, password);
                    var principal = new ClaimsPrincipal(identities);
                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                }
                else
                {
                    await RenderLoginPageAsync(context, userName, password, "Invalid user name or password!");
                }
            }
        }

        private static Task RenderLoginPageAsync(HttpContext context, string userName, string password, string errorMessage)
        {

            context.Response.ContentType = "text/html";
            return context.Response.WriteAsync(
                @"<html>
                <head><title>Login</title></head>
                <body>" +
                    $"<form method='post'>" +
                        $"<input type='text' name='username' placeholder='User name' value = '{userName}' /> " +
                        $"<input type='password' name='password' placeholder='Password' value = '{password}' /> " +
                       @"<input type='submit' value='Sign In' />
                    </form>" +
                        $"<p style='color:red'>{errorMessage}</p>" +
                    @"</body>
            </html>");
        }


        public static async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync();
            context.Response.Redirect("/");
        }

        static string FormatClaimPrincipal(ClaimsPrincipal principal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<dl>");
            sb.Append($"<dt>IsAuthenticated：{principal.Identity.IsAuthenticated}</dt>");
            sb.Append($"<dt>Identity Count ：{principal.Identities.Count()}</dt>");
            
            foreach (var identity in principal.Identities)
            {
                sb.Append(@$"<dt>Identity Name= {identity.Name} ; Label= {identity.Label} </dt>");
                foreach (var claim in identity.Claims)
                {
                    sb.Append($"<dd>{claim.Type}：{claim.Value}</dd>");
                }
                
            }

            sb.Append("<dt>principal.Claims</dt>");
            foreach (var principalClaim in principal.Claims)
            {
                sb.Append($"<dd>{principalClaim.Type}：{principalClaim.Value}</dd>");
            }

            sb.Append($"<dt>InRole：{principal.IsInRole("admin")}</dt>");
            sb.Append($"<dt>OwnerInRole：{principal.IsInRole("owner")}</dt>");
            sb.Append("</dl>");
            return sb.ToString();
        }
    }

    
}
