namespace Ecommerce.BFF.Models
{
    
    
    
    public class TokenSet
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Scope { get; set; }
    }

    
    
    
    public class UserInfo
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public List<string> Roles { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
    }
}
