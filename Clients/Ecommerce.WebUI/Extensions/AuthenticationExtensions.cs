using Microsoft.AspNetCore.Authentication.Cookies;

namespace Ecommerce.WebUI.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthenticationServices(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
            {
                opt.LoginPath = "/Login/Index/";
                opt.ExpireTimeSpan = TimeSpan.FromDays(5);
                opt.SlidingExpiration = true;
                opt.LogoutPath = "/Login/Logout/";
                opt.AccessDeniedPath = "/Error/AccessDenied/";
                opt.Cookie.HttpOnly = true;
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opt.Cookie.Name = "EcommerceCookie";
            });
        }
    }
}
