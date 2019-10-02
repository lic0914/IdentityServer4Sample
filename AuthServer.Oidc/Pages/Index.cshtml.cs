using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Oidc.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!Identity.IsLogin)
                return RedirectToPage("Login");
            return Page();
        }
    }
}
