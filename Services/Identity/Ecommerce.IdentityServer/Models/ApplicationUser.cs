using Microsoft.AspNetCore.Identity;

namespace Ecommerce.IdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
