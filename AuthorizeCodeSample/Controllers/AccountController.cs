using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.Models;

namespace MvcCookieAuthSample.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestUserStore _users;
        public AccountController(TestUserStore users)
        {
            _users = users;
        }
        private IActionResult RedirectToLoacl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                
                var user = _users.FindByUsername(vm.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(vm.UserName), "name not exists");
                }
                else
                {
                    if (_users.ValidateCredentials(vm.UserName, vm.Password))
                    {
                        var props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                        };
                        await Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(
                             HttpContext,
                             user.SubjectId,
                             user.Username,
                             props);
                        // HttpContext.SignInAsync(user.SubjectId,user.Username,)

                        return RedirectToLoacl(returnUrl);
                    }
                    ModelState.AddModelError(nameof(vm.Password), "password is wrong");
                }
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}