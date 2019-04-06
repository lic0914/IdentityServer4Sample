using System;
using System.Collections;
using System.Collections.Generic;

namespace MvcCookieAuthSample
{
    public class InputConsentViewModel{
        public string Button { get; set; }  
        public string ReturnUrl { get; set; }
        public bool RememberConsent { get;  set; }
        public IEnumerable<string> ScopesConsented { get;  set; }
    }

}