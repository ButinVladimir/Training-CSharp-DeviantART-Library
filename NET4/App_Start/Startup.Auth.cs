using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Owin.Security.Providers.DeviantArt;
using Owin.Security.Providers.DeviantArt.Provider;

namespace NET4
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ApplicationCookie);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = "AspNet.Auth.Cookie"
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseDeviantArtAuthentication(new DeviantArtAuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["deviantART:clientId"],
                ClientSecret = ConfigurationManager.AppSettings["deviantART:clientSecret"],
                Provider = new DeviantArtAuthenticationProvider()
                {
                    OnAuthenticated = context =>
                    {
                        context.Identity.AddClaim(new Claim("urn:DeviantArt:access_token", context.AccessToken));

                        return Task.FromResult(true);
                    }
                }
            });
        }
    }
}