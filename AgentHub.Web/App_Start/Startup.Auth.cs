using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using AgentHub.Entities.Utilities;
using AgentHub.Web.Identity;
using AgentHub.Web.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace AgentHub.Web
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Gets the o authentication options.
        /// </summary>
        /// <value>
        /// The o authentication options.
        /// </value>
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        /// <summary>
        /// Gets the o authentication bearer options.
        /// </summary>
        /// <value>
        /// The o authentication bearer options.
        /// </value>
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public static GoogleOAuth2AuthenticationOptions GoogleAuthOptions { get; private set; }
        
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

        /// <summary>
        /// Gets the public client identifier.
        /// </summary>
        /// <value>
        /// The public client identifier.
        /// </value>
        public static string PublicClientId { get; private set; }

        /// <summary>
        /// Configures the authentication.
        /// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        /// </summary>
        /// <param name="app">The application.</param>
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Token Generation
            if (OAuthBearerOptions == null)
            {
                OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
                app.UseOAuthBearerAuthentication(OAuthBearerOptions);
            }

            if (OAuthOptions == null)
            {
                PublicClientId = "self";
                OAuthOptions = new OAuthAuthorizationServerOptions()
                {
                    AllowInsecureHttp = AppSettings.AllowInsecureHttp,
                    TokenEndpointPath = new PathString("/Token"),
                    AccessTokenExpireTimeSpan = TimeSpan.FromHours(AppSettings.AccessTokenExpirationInHours),
                    Provider = new ApplicationOAuthProvider(PublicClientId)
                };
                app.UseOAuthAuthorizationServer(OAuthOptions);
            }
#if DEBUG
            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
#endif
            GoogleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = AppSettings.GoogleClientId,
                ClientSecret = AppSettings.GoogleClientSecret,
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(GoogleAuthOptions);
#if DEBUG
            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
#endif

            FacebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = AppSettings.FacebookAppId,
                AppSecret = AppSettings.FacebookAppSecret,
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(FacebookAuthOptions);
#if DEBUG
            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
#endif

        }
    }
}
