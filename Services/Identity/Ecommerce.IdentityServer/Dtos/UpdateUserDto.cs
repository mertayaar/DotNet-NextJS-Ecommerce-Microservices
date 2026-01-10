using System;

namespace Ecommerce.IdentityServer.Dtos
{
    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
