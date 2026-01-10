using System;

namespace Ecommerce.IdentityServer.Tools
{
    public class TokenResponseViewModel
    {
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }

        public TokenResponseViewModel(string token, DateTime expiredDate)
        {
            Token = token;
            ExpiredDate = expiredDate;
        }
    }
}
