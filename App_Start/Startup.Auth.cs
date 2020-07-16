using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(MiniShopWebNHT.App_Start.Startup))]

namespace MiniShopWebNHT.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/TaiKhoan/DangNhap"),
                SlidingExpiration = true
            });
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "134741838966-ehecbpvl0hus8kdna0v22suc1eao5ltf.apps.googleusercontent.com",
                ClientSecret = "a615LUwz_LAtmDX_dMUXDiEZ",
                CallbackPath = new PathString("/GoogleLoginCallback")
            });
        }
    }
}
