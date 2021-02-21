using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Pages
{
    public class LogoutModel : PageModel
    {
       

        public IActionResult OnGet()
        {
            return RedirectToPage("/Login");
        }
        
       
    }

  
}