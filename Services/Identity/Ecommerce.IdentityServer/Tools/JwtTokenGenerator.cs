using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.IdentityServer.Tools
{
    public class JwtTokenGenerator
    {
        public static TokenResponseViewModel GenerateToken(GetCheckAppUserViewModel userViewModel)
        {
            var claims = new List<Claim>();
            if(!string.IsNullOrEmpty(userViewModel.Role))
                claims.Add(new Claim(ClaimTypes.Role, userViewModel.Role));
           
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userViewModel.Id));

            if(!string.IsNullOrWhiteSpace(userViewModel.Username))
                claims.Add(new Claim("Username", userViewModel.Username));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenDefaults.Key));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireDate = DateTime.UtcNow.AddMinutes(JwtTokenDefaults.ExpireMinutes);
            JwtSecurityToken token = new JwtSecurityToken(issuer: JwtTokenDefaults.ValidIssuer,
                audience: JwtTokenDefaults.ValidAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireDate,
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return new TokenResponseViewModel(tokenHandler.WriteToken(token), expireDate);

        }
    }
}
