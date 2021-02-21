using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MixAuthSample.Pages
{
    public class LoginModel : PageModel
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
        public void OnGet()
        {

        }
        public void OnPost()
        {
           
        }
    }
}
