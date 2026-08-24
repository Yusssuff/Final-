using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Final_Task.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

namespace Final_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        private readonly IPasswordHasher<User>
            _passwordHasher;

        private readonly IConfiguration
            _configuration;

        public AuthController(
            AppDbContext db,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // =========================================
        // REGISTER
        // =========================================

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message =
                        "Request body is required."
                });
            }

            var username =
                request.Username?.Trim();

            var roleName =
                request.Role?.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new
                {
                    message =
                        "Username is required."
                });
            }

            if (string.IsNullOrWhiteSpace(
                request.Password))
            {
                return BadRequest(new
                {
                    message =
                        "Password is required."
                });
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new
                {
                    message =
                        "Password must be at least 6 characters."
                });
            }

            if (string.IsNullOrWhiteSpace(
                request.ConfirmPassword))
            {
                return BadRequest(new
                {
                    message =
                        "Confirm password is required."
                });
            }

            if (request.Password !=
                request.ConfirmPassword)
            {
                return BadRequest(new
                {
                    message =
                        "Passwords do not match."
                });
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest(new
                {
                    message =
                        "Role is required."
                });
            }

            // Check if username already exists.

            var exists =
                await _db.Users.AnyAsync(
                    u => u.Username == username);

            if (exists)
            {
                return Conflict(new
                {
                    message =
                        "Username already exists."
                });
            }

            // Find role by name.
            // The frontend sends "Admin" or "User",
            // NOT RoleId.

            var role =
                await _db.Roles
                    .FirstOrDefaultAsync(
                        r => r.Name == roleName);

            if (role == null)
            {
                return BadRequest(new
                {
                    message =
                        "Role not found."
                });
            }

            // Create user.

            var user = new User
            {
                Username = username,

                // RoleId is assigned internally.
                RoleId = role.Id
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.Password);

            await _db.Users.AddAsync(user);

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Registration successful.",

                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    role = role.Name
                }
            });
        }

        // =========================================
        // LOGIN
        // =========================================

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] AuthRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message =
                        "Request body is required."
                });
            }

            var username =
                request.Username?.Trim();

            if (
                string.IsNullOrWhiteSpace(username)
                ||
                string.IsNullOrWhiteSpace(
                    request.Password))
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid username or password."
                });
            }

            // Load User + Role + Permissions.

            var user =
                await _db.Users
                    .Include(u => u.Role)
                    .ThenInclude(
                        r => r!.RolePermissions)
                    .FirstOrDefaultAsync(
                        u => u.Username == username);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid username or password."
                });
            }

            if (user.Role == null)
            {
                return Unauthorized(new
                {
                    message =
                        "User role is not configured."
                });
            }

            // Verify password.

            var passwordResult =
                _passwordHasher
                    .VerifyHashedPassword(
                        user,
                        user.PasswordHash,
                        request.Password);

            if (
                passwordResult ==
                PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid username or password."
                });
            }

            // Generate JWT.

            var token =
                GenerateToken(user);

            // Return permissions to frontend.

            var permissions =
                user.Role.RolePermissions
                    .Select(p => new
                    {
                        operation =
                            p.Operation,

                        permission =
                            p.Permission.ToString()
                    })
                    .ToList();

            return Ok(new
            {
                message =
                    "Login successful.",

                token,

                tokenType =
                    "Bearer",

                expiresIn =
                    8 * 60 * 60,

                user = new
                {
                    id = user.Id,

                    username =
                        user.Username,

                    role =
                        user.Role.Name
                },

                permissions
            });
        }

        // =========================================
        // ME
        // =========================================

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                authenticated =
                    User.Identity?.IsAuthenticated
                    ?? false,

                userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier),

                username =
                    User.FindFirstValue(
                        ClaimTypes.Name),

                role =
                    User.FindFirstValue(
                        ClaimTypes.Role)
            });
        }

        // =========================================
        // GET ROLES
        // =========================================

        [AllowAnonymous]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles =
                await _db.Roles
                    .OrderBy(r => r.Name)
                    .Select(r => new
                    {
                        id = r.Id,
                        name = r.Name
                    })
                    .ToListAsync();

            return Ok(roles);
        }

        // =========================================
        // GENERATE JWT
        // =========================================

        private string GenerateToken(User user)
        {
            var jwtKey =
                _configuration["JWT:Key"];

            var issuer =
                _configuration["JWT:ValidIssuer"];

            var audience =
                _configuration["JWT:ValidAudience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT:Key is missing."
                );
            }

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new InvalidOperationException(
                    "JWT:ValidIssuer is missing."
                );
            }

            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "JWT:ValidAudience is missing."
                );
            }

            // Role object -> role NAME string.

            var roleName =
                user.Role?.Name
                ?? string.Empty;

            var claims =
                new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()
                    ),

                    new System.Security.Claims.Claim(
                        ClaimTypes.Name,
                        user.Username
                    ),

                    new System.Security.Claims.Claim(
                        ClaimTypes.Role,
                        roleName
                    ),

                    new System.Security.Claims.Claim(
                        "role_id",
                        user.RoleId.ToString()
                    ),

                    new System.Security.Claims.Claim(
                        JwtRegisteredClaimNames.Sub,
                        user.Id.ToString()
                    ),

                    new System.Security.Claims.Claim(
                        JwtRegisteredClaimNames.UniqueName,
                        user.Username
                    ),

                    new System.Security.Claims.Claim(
                        JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString()
                    )
                };

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                );

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

            var token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires:
                        DateTime.UtcNow.AddHours(8),
                    signingCredentials:
                        credentials
                );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}