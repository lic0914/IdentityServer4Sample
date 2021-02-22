using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtBearer.Samples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly JwtSettings _settings;

        public TokenController(JwtSettings settings)
        {
            _settings = settings;
        }
        [HttpGet]
        public IActionResult Token(string name)
        {
            if (name != "lic")
            {
                return BadRequest();
            }

            var claims=new Claim[]
            {
               new Claim("name",name),
               new Claim("role","admin"),
               new Claim("actor","boss"), 
               new Claim("email","lic@qq.com"), 
            };

           var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
           
            var token=new JwtSecurityToken(_settings.Issuer,_settings.Audience,claims,DateTime.Now,DateTime.Now.AddMinutes(30)
            ,creds);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}