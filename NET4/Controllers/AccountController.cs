using System;
using System.Net;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin.Security.Providers.DeviantArt;
using Owin.Security;
using Owin;
using Net4.Models;

namespace Net4._5.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        const string CallbackAction = "Callback";

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return new ChallengeResult(Url.Action(CallbackAction));
        }

        //
        // GET: /Account/Logout
        public ActionResult Logout()
        {
            this.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Callback
        [AllowAnonymous]
        public async Task<ActionResult> Callback()
        {
            IAuthenticationManager authenticationManager = this.HttpContext.GetOwinContext().Authentication;
            AuthenticateResult authenticateResult = await authenticationManager.AuthenticateAsync("ExternalCookie");

            if (authenticateResult != null)
            {
                var claims = authenticateResult.Identity.Claims;
                string userId = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                string userName = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name).Value;
                string accessToken = claims.FirstOrDefault(claim => claim.Type == "urn:DeviantArt:access_token").Value;

                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    UserManager.Create(new ApplicationUser()
                    {
                        Id = userId,
                        UserName = userName,
                        AccessToken = accessToken
                    });
                }
                else
                {
                    user.AccessToken = accessToken;
                    user.UserName = userName;

                    UserManager.Update(user);
                }

                ExternalLoginInfo userInfo = await authenticationManager.GetExternalLoginInfoAsync();

                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                await UserManager.AddLoginAsync(userId, userInfo.Login);

                ClaimsIdentity identity = await user.GenerateUserIdentityAsync(UserManager);
                authenticationManager.SignIn(
                    new AuthenticationProperties()
                    {
                        IsPersistent = true
                    },
                    identity);
            }

            return RedirectToAction("Index", "Home");
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string redirectUri)
            {
                this.RedirectUri = redirectUri;
            }

            public string RedirectUri { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var authProperties = new AuthenticationProperties()
                {
                    RedirectUri = this.RedirectUri
                };

                context.HttpContext.GetOwinContext().Authentication.Challenge(authProperties, "DeviantArt");
            }
        }
    }
}