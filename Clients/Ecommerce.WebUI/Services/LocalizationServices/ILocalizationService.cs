using Microsoft.Extensions.Localization;

namespace Ecommerce.WebUI.Services.LocalizationServices
{
    public interface ILocalizationService
    {
       public LocalizedString GetLocalizeString(string key);
    }
}
