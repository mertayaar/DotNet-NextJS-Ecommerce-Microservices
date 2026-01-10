using Ecommerce.IdentityServer.Dtos;
using Ecommerce.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Ecommerce.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegistersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        
        
        
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UserRegister(UserRegisterDto userRegisterDto)
        {
            var values = new ApplicationUser()
            {
                UserName = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                Name = userRegisterDto.Name,
                Surname = userRegisterDto.Surname,
            };
            
            var result = await _userManager.CreateAsync(values, userRegisterDto.Password);
            
            if (result.Succeeded)
            {
                
                if (!await _roleManager.RoleExistsAsync("Customer"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                }

                
                await _userManager.AddToRoleAsync(values, "Customer");

                return Ok(new { success = true, message = "Registration successful" });
            }
            else
            {
                return BadRequest(new { success = false, errors = result.Errors });
            }
        }

        
        
        
        
        
        
        [Authorize(LocalApi.PolicyName, Roles = "Admin")]
        [HttpPost("assign-admin/{username}")]
        public async Task<IActionResult> AssignAdminRole(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { success = false, message = $"User '{username}' not found" });
            }

            
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Ok(new { success = true, message = $"User '{username}' is already an Admin" });
            }

            
            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                return Ok(new { success = true, message = $"User '{username}' is now an Admin" });
            }

            return BadRequest(new { success = false, errors = result.Errors });
        }

        
        
        
        
        
        
        [Authorize(LocalApi.PolicyName, Roles = "Admin")]
        [HttpDelete("remove-admin/{username}")]
        public async Task<IActionResult> RemoveAdminRole(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { success = false, message = $"User '{username}' not found" });
            }

            
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return BadRequest(new { success = false, message = $"User '{username}' is not an Admin" });
            }

            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.UserName == username)
            {
                return BadRequest(new { success = false, message = "Cannot remove Admin role from yourself" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                return Ok(new { success = true, message = $"Admin role removed from '{username}'" });
            }

            return BadRequest(new { success = false, errors = result.Errors });
        }

        
        
        
        
        [Authorize(LocalApi.PolicyName, Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    id = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    name = user.Name,
                    surname = user.Surname,
                    roles = roles
                });
            }

            return Ok(new { success = true, users = userList });
        }
    }
}
