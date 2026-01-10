using Ecommerce.Common;
using Ecommerce.WebUI.Models;
using Ecommerce.WebUI.Services.Interfaces;

namespace Ecommerce.WebUI.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
            
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDetailViewModel?> GetUserInfo()
        {
            var userInfo = await _httpClient.GetFromJsonAsync<ApiResponse<UserDetailViewModel>>("/api/users/getuser");
            return userInfo.Data;
        }
    }
}
