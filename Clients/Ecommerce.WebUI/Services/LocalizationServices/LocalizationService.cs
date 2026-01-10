using Ecommerce.WebUI.Resources;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Ecommerce.WebUI.Services.LocalizationServices
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory localizerFactory)
        {
            var type = typeof(AppResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!);
            _localizer = localizerFactory.Create("AppResource", assemblyName.Name!);
        }

        public LocalizedString GetLocalizeString(string key)
        {
            var result = _localizer[key];
            return result;
        }
    }
}
