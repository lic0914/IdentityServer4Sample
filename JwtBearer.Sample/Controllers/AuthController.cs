using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtBearer.Sample.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        
        public IActionResult Token(string name,string pwd)
        {
            if (name != "lic")
            {
                return BadRequest();
            }

            var claims=new Claim[]
            {
               new Claim(ClaimTypes.Name,name),
               new Claim(ClaimTypes.Role,"admin"),
            };

           var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.JwtSecret));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
           
            var token=new JwtSecurityToken("http://localhost:5200","api",claims,DateTime.Now,DateTime.Now.AddMinutes(30)
            ,creds);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}