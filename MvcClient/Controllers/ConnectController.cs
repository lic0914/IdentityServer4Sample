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
    public class ConnectController : ControllerBase
    {
     
        [HttpGet("token")]
        public async Task<string> Token()
        {
            using(var http=new HttpClient())
            {
                var body = JsonConvert.SerializeObject(new {
                    client_id="client",
                    client_secret="secret",
                    grant_type="client_credentials"
                });
                var kvParas = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("client_id","client"),
                    new KeyValuePair<string, string>("client_secret","secret"),
                    new KeyValuePair<string, string>("grant_type","client_credentials"),
                };
                HttpContent cont = new FormUrlEncodedContent(kvParas);
                var res =await http.PostAsync("http://localhost:5000/connect/token", cont);
                    
                res.EnsureSuccessStatusCode();
                return await res.Content.ReadAsStringAsync();
                
            }
           
             
        }

    }
}
