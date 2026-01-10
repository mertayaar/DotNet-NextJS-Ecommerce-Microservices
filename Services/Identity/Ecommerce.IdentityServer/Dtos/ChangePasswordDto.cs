using System;
using Newtonsoft.Json;

namespace Ecommerce.IdentityServer.Dtos
{
    public class ChangePasswordDto
    {
        [JsonProperty("currentPassword")]
        public string CurrentPassword { get; set; }
        
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}
