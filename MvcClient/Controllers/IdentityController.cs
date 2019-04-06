using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MvcClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public dynamic Get()
        {
            return User.Claims.Select(e=>new { e.Type, e.Value });
        }
        [HttpGet("token")]
        public string Token()
        {
            using(var http=new HttpClient())
            {
                var body = JsonConvert.SerializeObject(new {
                    client_id="client",
                    client_secret="secret",
                    grant_type="client_credentials"
                });
               var res= http.PostAsync("http://localhost:5000/connect/token", new StringContent(body, Encoding.UTF8, "application/json"))
                    .Result;
                res.EnsureSuccessStatusCode();
                return res.Content.ReadAsStringAsync().Result;
                
            }
        }

    }
}
