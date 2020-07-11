using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Logout()
        {
            return SignOut("Cookies", "OpenIdConnect");
        }
    }
}
