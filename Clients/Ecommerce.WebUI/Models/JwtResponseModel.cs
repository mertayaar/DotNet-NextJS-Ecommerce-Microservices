namespace Ecommerce.WebUI.Models
{
    public class JwtResponseModel
    {
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
