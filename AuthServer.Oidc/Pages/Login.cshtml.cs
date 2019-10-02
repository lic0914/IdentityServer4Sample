using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Oidc.Pages
{
    public class LoginModel : PageModel
    {
       

        public void OnGet()
        {
            
        }
        [BindProperty]
        public LoginViewModel ViewModel { get; set; }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (!Identity.Login(ViewModel.UserName, ViewModel.Password))
            {
                ModelState.AddModelError("", "用户名或密码不正确");
                return Page();
            }

            return RedirectToPage("/Index");

        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        
        public string UserName { get; set; }
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }
}