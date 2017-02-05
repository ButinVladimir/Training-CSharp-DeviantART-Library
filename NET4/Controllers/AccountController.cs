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
using Net4._5.Models;

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
        // GET: /Account/Callback
        [AllowAnonymous]
        public async Task<ActionResult> Callback(string code)
        {
            this.ControllerContext.HttpContext.GetOwinContext().Authentication.SignIn();
            var result = this.ControllerContext.HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
            var result2 = this.ControllerContext.HttpContext.GetOwinContext().Authentication.GetExternalIdentity("DeviantArt");
            var result3 = this.ControllerContext.HttpContext.GetOwinContext().Environment;
            var result4 = this.ControllerContext.HttpContext.GetOwinContext().Request;
            var result6 = this.HttpContext;
            var result7 = this.ControllerContext;
            var result5 = this.ControllerContext.HttpContext.GetOwinContext().Authentication.User;
            var result9 = await this.ControllerContext.HttpContext.GetOwinContext().Authentication.AuthenticateAsync("DeviantArt");

            return null;
        }

        private class ChallengeResult: HttpUnauthorizedResult
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