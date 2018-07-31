using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;

using _2StepVerification.Providers;

[assembly: OwinStartup(typeof(_2StepVerification.Startup))]

namespace _2StepVerification
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static TimeSpan VerifiedAccessTokenExpireTimeSpan { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var myProvider = new ApplicationOAuthProvider();
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(10), //10 minutes for the expiration of login (unverified) token (which represents the verification code life time)
                Provider = myProvider
            };
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            VerifiedAccessTokenExpireTimeSpan = TimeSpan.FromHours(4); //The expiration of the verified token
        
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }
    }
}
