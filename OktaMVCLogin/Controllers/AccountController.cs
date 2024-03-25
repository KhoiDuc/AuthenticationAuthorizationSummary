using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Okta.AspNetCore;
using Microsoft.AspNetCore.Authorization;

namespace OktaMVCLogin.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Challenge(OktaDefaults.MvcAuthenticationScheme);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult SignOut()
        {
            return new SignOutResult(
                    new[]
                    {
                        OktaDefaults.MvcAuthenticationScheme,
                        CookieAuthenticationDefaults.AuthenticationScheme,
                    },
                    new AuthenticationProperties { RedirectUri = "/Account/SignIn" });
        }

        [HttpPost]
        public IActionResult SignOutTest()
        {
            return new SignOutResult(
               new[]
               {
         OktaDefaults.MvcAuthenticationScheme,
         CookieAuthenticationDefaults.AuthenticationScheme,
               },
               new AuthenticationProperties { RedirectUri = "/Home/" });
        }
    }
}
