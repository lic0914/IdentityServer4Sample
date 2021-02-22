using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JwtBearer.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logger => logger
                        .AddConsole().SetMinimumLevel(LogLevel.Debug))
                .ConfigureWebHostDefaults(builder => builder
                    .UseStartup<Startup>()
                    
                )
                .Build()
                .Run();



        }


    }
}

       
