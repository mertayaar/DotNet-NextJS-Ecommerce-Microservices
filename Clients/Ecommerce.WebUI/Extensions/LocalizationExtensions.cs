using System.Globalization;
using System.Reflection;
using Ecommerce.WebUI.Resources;
using Ecommerce.WebUI.Services.LocalizationServices;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Ecommerce.WebUI.Extensions
{
    public static class LocalizationExtensions
    {
        public static void AddLocalizationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(opt =>
                {
                    opt.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(AppResource).Assembly.FullName!);
                        return factory.Create("AppResource", assemblyName.Name!);
                    };
                });

            services.Configure<RequestLocalizationOptions>(opt =>
            {
                var cultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("tr-TR"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("es-ES"),
                    new CultureInfo("de-DE")
                };
                opt.DefaultRequestCulture = new RequestCulture("en-US");
                opt.SupportedCultures = cultures;
                opt.SupportedUICultures = cultures;

                opt.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });

            services.AddScoped<ILocalizationService, LocalizationService>();
        }
    }
}
