using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Final_Task.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using SalesBuzz.Shared.Authorization;

namespace Final_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPermissions _permissions;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IPermissions permissions,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _permissions = permissions;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user
            var user = await _userManager.FindByNameAsync(
                request.Username);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            // Validate password
            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    request.Password);

            if (!passwordValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            // Get user's roles
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                return Unauthorized(new
                {
                    message = "User has no assigned role."
                });
            }

            // Current application roles:
            // Admin / User
            var role = roles[0];

            // Load SalesBuzz permissions for this role
            _permissions.UpdateUserPermissions(role);

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName ?? request.Username),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };

            // Add all roles
            foreach (var userRole in roles)
            {
                if (userRole != role)
                {
                    claims.Add(
                        new Claim(
                            ClaimTypes.Role,
                            userRole));
                }
            }

            // Read JWT settings
            var jwtKey = _configuration["JWT:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "JWT:Key is missing from configuration."
                    });
            }

            /*
             * IMPORTANT:
             *
             * Use the exact Microsoft.IdentityModel.Tokens
             * assembly referenced by the SalesBuzz SDK.
             */
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

            var securityKey =
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    keyBytes);

            var signingCredentials =
                new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    securityKey,
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            // Generate JWT
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: signingCredentials);

            var tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                userId = user.Id,
                username = user.UserName,
                role = role
            });
        }
    }
}