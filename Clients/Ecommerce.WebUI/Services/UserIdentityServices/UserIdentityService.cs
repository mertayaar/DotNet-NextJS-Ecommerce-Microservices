using Ecommerce.Common;
using Ecommerce.DtoLayer.CatalogDtos.AboutDtos;
using Ecommerce.DtoLayer.IdentityDtos.UserDtos;

namespace Ecommerce.WebUI.Services.UserIdentityServices
{
    public class UserIdentityService : IUserIdentityService
    {

        private readonly HttpClient _httpClient;

        public UserIdentityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<ResultUserDto>> GetAllUserListAsync()
        {
            var responseMessage = await _httpClient.GetAsync("api/users/GetAllUsers");
            var values = await responseMessage.Content.ReadFromJsonAsync<List<ResultUserDto>>();
            return values;
        }
    }
}
