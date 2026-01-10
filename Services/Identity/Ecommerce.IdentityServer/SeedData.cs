



using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Ecommerce.IdentityServer.Data;
using Ecommerce.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Ecommerce.IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    
                    if (!roleMgr.RoleExistsAsync("Admin").Result)
                    {
                        roleMgr.CreateAsync(new IdentityRole("Admin")).Wait();
                        Log.Debug("Admin role created");
                    }
                    if (!roleMgr.RoleExistsAsync("Customer").Result)
                    {
                        roleMgr.CreateAsync(new IdentityRole("Customer")).Wait();
                        Log.Debug("Customer role created");
                    }

                    
                    var admin = userMgr.FindByNameAsync("admin").Result;
                    if (admin == null)
                    {
                        admin = new ApplicationUser
                        {
                            UserName = "admin",
                            Email = "admin@ecommerce.local",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(admin, "Admin123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(admin, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Admin User"),
                            new Claim(JwtClaimTypes.GivenName, "Admin"),
                            new Claim(JwtClaimTypes.FamilyName, "User"),
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        
                        userMgr.AddToRoleAsync(admin, "Admin").Wait();
                        Log.Debug("admin user created with Admin role");
                    }
                    else
                    {
                        Log.Debug("admin already exists");
                    }

                    
                    var customer = userMgr.FindByNameAsync("customer").Result;
                    if (customer == null)
                    {
                        customer = new ApplicationUser
                        {
                            UserName = "customer",
                            Email = "customer@ecommerce.local",
                            EmailConfirmed = true
                        };
                        var result = userMgr.CreateAsync(customer, "Customer123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(customer, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Customer User"),
                            new Claim(JwtClaimTypes.GivenName, "Customer"),
                            new Claim(JwtClaimTypes.FamilyName, "User"),
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        
                        userMgr.AddToRoleAsync(customer, "Customer").Wait();
                        Log.Debug("customer user created with Customer role");
                    }
                    else
                    {
                        Log.Debug("customer already exists");
                    }
                }
            }
        }
    }
}
