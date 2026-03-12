using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[assembly: OwinStartup(typeof(LocationTrackingAPI.Startup))]

namespace LocationTrackingAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        //public void ConfigureAuth(IAppBuilder app)
        //{
        //    var secretKey = "ThisIsMyVeryStrongSecretKeyForJwtToken123!";

        //    app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
        //    {
        //        AuthenticationMode = AuthenticationMode.Active,
        //        TokenValidationParameters = new TokenValidationParameters()
        //        {
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(
        //                Encoding.UTF8.GetBytes(secretKey))
        //        }
        //    });
        //}
    }
}



