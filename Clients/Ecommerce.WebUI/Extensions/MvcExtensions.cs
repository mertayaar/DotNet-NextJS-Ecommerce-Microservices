using Ecommerce.WebUI.Handlers;
using Ecommerce.WebUI.Services.Concrete;
using Ecommerce.WebUI.Services.Interfaces;
using Ecommerce.WebUI.Services.UserIdentityServices;
using Ecommerce.WebUI.Settings;

namespace Ecommerce.WebUI.Extensions
{
    public static class MvcExtensions
    {
        public static void AddMvcServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddHttpClient(); // Basic HttpClient
            services.AddControllersWithViews();

            services.Configure<ClientSettings>(configuration.GetSection("ClientSettings"));
            services.Configure<ServiceApiSettings>(configuration.GetSection("ServiceApiSettings"));

            services.AddScoped<ResourceOwnerPasswordTokenHandler>();
            services.AddScoped<ClientCredentialTokenHandler>();
        }
    }
}
