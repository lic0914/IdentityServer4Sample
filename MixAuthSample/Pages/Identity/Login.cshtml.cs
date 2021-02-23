using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MixAuthSample.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string UserName { get; set; } = "lic";

        [BindProperty]
        public string Password { get; set; } = "123";
        public IActionResult OnGet()
        {
            if (HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                return Redirect("/");
            }

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (UserName == "lic" && Password == "123")
            {
                var identities = new List<ClaimsIdentity>
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
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,new AuthenticationProperties()
                {
                    //源码 https://source.dot.net/#Microsoft.AspNetCore.Authentication.Cookies/CookieAuthenticationHandler.cs,7ff3e563b061958e,references
                    IsPersistent = true //持久保存cookie 浏览器关闭后不清除cookie 
                });
                return Redirect("/");
            }

            return Page();
        }
    }
}
