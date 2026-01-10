using Ecommerce.BFF.Models;
using System.Threading.Tasks;

namespace Ecommerce.BFF.Services
{
    
    
    
    
    public interface ITokenStore
    {
        
        
        
        Task StoreTokensAsync(string sessionId, TokenSet tokens);

        
        
        
        Task<TokenSet> GetTokensAsync(string sessionId);

        
        
        
        Task RemoveTokensAsync(string sessionId);

        
        
        
        Task UpdateTokensAsync(string sessionId, TokenSet tokens);
    }
}
