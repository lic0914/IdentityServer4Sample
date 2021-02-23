using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtBearer.Samples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MixAuthSample.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly JwtSettings _settings;

        public TokenController(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public IActionResult Token(string name)
        {
            if (name != "lic")
            {
                return BadRequest();
            }

            var claims = new Claim[]
            {
                new Claim("name",name),
                new Claim("role","admin"),
                new Claim("actor","boss"),
                new Claim("email","lic@qq.com"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_settings.Issuer, _settings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(30)
                , creds);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpGet("Resolve")]
        public IActionResult Resolve(string token)
        {
            TokenValidationParameters parameter=new TokenValidationParameters()
            {
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret))
        };
            SecurityToken securityToken;
            var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, parameter, out securityToken);
            
            return Ok(claimsPrincipal.Identity);
        }
    }
}