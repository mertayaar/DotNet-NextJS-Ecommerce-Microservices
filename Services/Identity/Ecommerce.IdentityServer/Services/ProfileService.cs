using Ecommerce.IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.IdentityServer.Services
{
    
    
    
    
    
    
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        
        
        
        
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            
            if (user == null)
            {
                return;
            }

            
            var roles = await _userManager.GetRolesAsync(user);

            
            var roleClaims = roles.Select(role => new Claim(JwtClaimTypes.Role, role));
            context.IssuedClaims.AddRange(roleClaims);

            
            context.IssuedClaims.Add(new Claim(JwtClaimTypes.Subject, user.Id));
            
            
            if (!string.IsNullOrEmpty(user.UserName))
            {
                context.IssuedClaims.Add(new Claim("username", user.UserName));
            }
            
            
            var fullName = string.Join(" ", new[] { user.Name, user.Surname }.Where(s => !string.IsNullOrEmpty(s)));
            context.IssuedClaims.Add(new Claim(JwtClaimTypes.Name, !string.IsNullOrEmpty(fullName) ? fullName : user.UserName ?? user.Email));
            
            if (!string.IsNullOrEmpty(user.Email))
            {
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.Email, user.Email));
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower()));
            }

            
            if (!string.IsNullOrEmpty(user.Name))
            {
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.GivenName, user.Name));
            }

            
            if (!string.IsNullOrEmpty(user.Surname))
            {
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.FamilyName, user.Surname));
            }
        }

        
        
        
        
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            context.IsActive = user != null;
        }
    }
}
