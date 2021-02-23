using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MixAuthSample.Pages.Identity
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           return Redirect("/");
        }
    }
}
