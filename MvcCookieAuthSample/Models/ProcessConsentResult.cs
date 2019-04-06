using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.Models
{
    public class ProcessConsentResult
    {
        public string RedirectUrl { get; set; }
        public bool IsRedirect  => RedirectUrl != null;
        public ConsentViewModel ConsentViewModel { get; set; }

        public string ValidationError { get; set; }
    }
}
