using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace OktaNet4SSOLogin.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(OpenIdConnectAuthenticationDefaults.AuthenticationType);
                return new HttpUnauthorizedResult();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]

        public ActionResult LoginDefault()
        {
            if (IsValidLogin("admin", "admin"))
            {
                // Create claims for the authenticated user
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, "adminTest"),
                   new Claim("LoginType", "Default"),
                    // Add more claims as needed
                }, CookieAuthenticationDefaults.AuthenticationType);

                // Sign in the user using cookie authentication
                HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false }, // Set IsPersistent based on your requirements
                    identity
                );

                // Redirect the user to the requested URL or the home page
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        private bool IsValidLogin(string username, string password)
        {
            // Validate the provided credentials against your authentication mechanism
            // For demonstration purposes, let's assume the login is successful if the username and password are both "admin"
            return username == "admin" && password == "admin";
        }
        private bool IsAdmin()
        {
            return ClaimsPrincipal.Current.IsInRole("");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;

                // Check for claim with "LoginType"
                var loginTypeClaim = claimsIdentity.FindFirst(ClaimTypes.AuthenticationMethod);

                if (loginTypeClaim != null && loginTypeClaim.Value == "Default")
                {
                    // Use CookieAuthenticationDefaults.AuthenticationType for default login
                    HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                }
                else
                {
                    // Use OpenIdConnectAuthenticationDefaults.AuthenticationType for other login types
                    HttpContext.GetOwinContext().Authentication.SignOut(
                        OpenIdConnectAuthenticationDefaults.AuthenticationType,
                        CookieAuthenticationDefaults.AuthenticationType);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult PostLogout()
        {
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Claims()
        {
            return View(HttpContext.GetOwinContext().Authentication.User.Claims);
        }
    }
}