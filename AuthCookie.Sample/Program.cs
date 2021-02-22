using System;
using System.Collections.Generic;
using System.Security.Claims;
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
                foreach (var userClaim in context.User.Claims)
                {
                    body += $"<p>{userClaim.Type}:{userClaim.Value}</p>";
                }
                body += "<a href='Account/Logout'>Sign Out</a>";
                await context.Response.WriteAsync(@"<html><head><title>Index</title></head> <body>" + body + "</body></html>");
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
                    var identity = new ClaimsIdentity(new List<Claim>()
                    {
                        new Claim(userName, password),
                        new Claim("eamil","11@qq.com")
                    }, "admin");
                    //var identity = new GenericIdentity(userName, password);
                    var principal = new ClaimsPrincipal(identity);
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
    }

    
}
