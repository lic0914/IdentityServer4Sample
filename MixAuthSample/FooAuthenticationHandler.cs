using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MixAuthSample
{
    public class FooAuthenticationOption : AuthenticationSchemeOptions
    {

    }
    public class FooAuthenticationHandler : AuthenticationHandler<FooAuthenticationOption>
    {
        private readonly ILogger<FooAuthenticationHandler> _logger;
        public FooAuthenticationHandler(IOptionsMonitor<FooAuthenticationOption> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<FooAuthenticationHandler>();
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            LogInformation($"InitializeAsync scheme:{scheme.Name}" );
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            LogInformation($"AuthenticateAsync");
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,"lic"),
                new Claim(ClaimTypes.Role,"role"),

            };
            var claimsIdentity = new ClaimsIdentity(claims);
            var result = AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity),"Foo"));
            return Task.FromResult(result);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            LogInformation($"HandleAuthenticateAsync");
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            LogInformation($"ChallengeAsync");
            return Task.CompletedTask;

        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            LogInformation($"ForbidAsync");
            return Task.CompletedTask;
        }
        public void LogInformation(string msg)
        {
            var action= LoggerMessage.Define<string>(
                eventId: new EventId(10000, "LogInformation"),
                logLevel: LogLevel.Debug,
                formatString: "Authenti: {msg} ");
            action(_logger, msg, null);
            _logger.LogDebug(typeof(FooAuthenticationHandler).FullName + "  " + msg);
        }

    }
}