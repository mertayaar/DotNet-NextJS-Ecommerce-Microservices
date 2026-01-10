


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Ecommerce.IdentityServer
{
    public static class Config
    {
        
        
        
        
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("catalog_api", "Catalog Service")
            {
                Scopes = { "catalog.read", "catalog.write" },
                UserClaims = { "role", "name", "email" }
            },
            new ApiResource("cart_api", "Shopping Cart Service")
            {
                Scopes = { "cart.manage" },
                UserClaims = { "role", "sub", "name" }
            },
            new ApiResource("order_api", "Order Management Service")
            {
                Scopes = { "order.read", "order.write", "order.admin" },
                UserClaims = { "role", "sub", "name", "email" }
            },
            new ApiResource("discount_api", "Discount Service")
            {
                Scopes = { "discount.read", "discount.write" },
                UserClaims = { "role" }
            },
            new ApiResource("payment_api", "Payment Service")
            {
                Scopes = { "payment.process" },
                UserClaims = { "role", "sub", "email" }
            },
            new ApiResource("review_api", "Review Service")
            {
                Scopes = { "review.read", "review.write" },
                UserClaims = { "role", "sub" }
            },
            new ApiResource("cargo_api", "Cargo/Shipping Service")
            {
                Scopes = { "cargo.read", "cargo.write" },
                UserClaims = { "role" }
            },
            new ApiResource("message_api", "Messaging Service")
            {
                Scopes = { "message.read", "message.write" },
                UserClaims = { "role", "sub" }
            },
            new ApiResource("image_api", "Image Service")
            {
                Scopes = { "image.upload", "image.read" },
                UserClaims = { "role" }
            },
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            {
                Scopes = { IdentityServerConstants.LocalApi.ScopeName }
            }
        };

        
        
        
        
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            
            
            new IdentityResource(
                name: "roles",
                userClaims: new[] { "role" },
                displayName: "User roles")
        };

        
        
        
        
        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            
            new ApiScope("catalog.read", "Read access to product catalog"),
            new ApiScope("catalog.write", "Write access to product catalog"),
            
            
            new ApiScope("cart.manage", "Manage shopping cart"),
            
            
            new ApiScope("order.read", "Read order information"),
            new ApiScope("order.write", "Create and modify orders"),
            new ApiScope("order.admin", "Administrative order operations"),
            
            
            new ApiScope("discount.read", "View discounts and coupons"),
            new ApiScope("discount.write", "Create and manage discounts"),
            
            
            new ApiScope("payment.process", "Process payments"),
            
            
            new ApiScope("review.read", "Read product reviews"),
            new ApiScope("review.write", "Write product reviews"),
            
            
            new ApiScope("cargo.read", "Track shipments"),
            new ApiScope("cargo.write", "Manage shipping information"),
            
            
            new ApiScope("message.read", "Read messages"),
            new ApiScope("message.write", "Send messages"),
            
            
            new ApiScope("image.upload", "Upload images"),
            new ApiScope("image.read", "View images"),
            
            
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName, "Access to IdentityServer API")
        };

        
        
        
        
        public static IEnumerable<Client> Clients => new Client[]
        {
            
            
            
            
            new Client
            {
                ClientId = "ecommerce_bff",
                ClientName = "Ecommerce BFF Service",
                
                
                
                
                
                AllowedGrantTypes = new List<string> { "authorization_code", "password" },
                RequirePkce = true,          
                RequireClientSecret = false, 
                
                
                RedirectUris = {
                    "http://localhost:5500/auth/callback",
                    "http://localhost:3000/backend/auth/callback"
                    
                },
                
                
                PostLogoutRedirectUris = {
                    "http://localhost:5500",
                    
                },
                
                
                AllowedCorsOrigins = {
                    "http://localhost:5500",
                    
                },
                
                
                AllowedScopes = {
                    
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "roles",
                    
                    
                    "catalog.read",
                    "cart.manage",
                    "order.read",
                    "order.write",
                    "discount.read",
                    "payment.process",
                    "review.read",
                    "review.write",
                    "cargo.read",
                    "message.read",
                    "message.write",
                    "image.read",
                    
                    
                    "catalog.write",
                    "discount.write",
                    "order.admin",
                    "cargo.write",
                    "image.upload",
                    
                    
                    IdentityServerConstants.LocalApi.ScopeName
                },
                
                
                AllowOfflineAccess = true,
                
                
                AccessTokenLifetime = 3600,        

                AbsoluteRefreshTokenLifetime = 2592000,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,  
                RefreshTokenExpiration = TokenExpiration.Absolute,
                
                
                RequireConsent = false,  
                AlwaysIncludeUserClaimsInIdToken = true,  
                
                
                AllowAccessTokensViaBrowser = true
            },
            
            
            
            
            
            
            new Client
            {
                ClientId = "ecommerce_public_catalog",
                ClientName = "Public Catalog Browser",
                
                
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("catalog-public-secret-change-in-production".Sha256()) },
                
                
                AllowedScopes = {
                    
                    "catalog.read",
                    "discount.read",
                    "review.read",
                    "image.read",
                    "order.read",
                    "cargo.read",
                    "message.read",
                    "cart.manage",
                    
                    
                    "catalog.write",
                    "discount.write",
                    "review.write",
                    "image.upload",
                    "order.write",
                    "order.admin",
                    "cargo.write",
                    "message.write",
                    
                    
                    IdentityServerConstants.LocalApi.ScopeName
                },
                
                
                AllowOfflineAccess = false,
                
                AccessTokenLifetime = 3600,
                
                
                Claims = {
                    new ClientClaim("client_type", "webui_admin"),
                    new ClientClaim("usage", "admin_panel")
                }
            }
        };
    }
}