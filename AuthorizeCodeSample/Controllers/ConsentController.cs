using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.Models;
using IdentityServer4.Models;
using MvcCookieAuthSample.Services;

namespace MvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consentSvc;
        public ConsentController(ConsentService consentSvc)
        {
            _consentSvc = consentSvc;
        }
        public async Task<IActionResult> Index(string returnUrl, InputConsentViewModel vm)
        {
            var model = await _consentSvc.BuildConsentViewModel(returnUrl, vm);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel vm)
        {

            var result = await _consentSvc.ProcessConsent(vm);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUrl);
            }
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }
            return View(result.ConsentViewModel);
        }
    }
}