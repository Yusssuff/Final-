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
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly SalesBuzzPermissionService _permissions;
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext db,
            IPasswordHasher<User> passwordHasher,
            SalesBuzzPermissionService permissions,
            IConfiguration configuration)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _permissions = permissions;
            _configuration = configuration;
        }

 
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Request body is required."
                });
            }

            var username = request.Username?.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new
                {
                    message = "Username is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Password is required."
                });
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new
                {
                    message = "Password must be at least 6 characters."
                });
            }

            var exists = await _db.Users
                .AnyAsync(x => x.Username == username);

            if (exists)
            {
                return Conflict(new
                {
                    message = "Username already exists."
                });
            }


            var user = new User
            {
                Username = username,
                Role = "User"
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.Password
                );

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();


            return Ok(new
            {
                message = "Registration successful.",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    role = user.Role
                }
            });
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Request body is required."
                });
            }

            var username = request.Username?.Trim();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password
                );

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }



            var token = GenerateToken(user);

            return Ok(new
            {
                message = "Login successful.",
                token = token,
                tokenType = "Bearer",
                expiresIn = 8 * 60 * 60,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    role = user.Role
                }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                authenticated =
                    User.Identity?.IsAuthenticated ?? false,

                userId = User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                ),

                username = User.FindFirstValue(
                    ClaimTypes.Name
                ),

                role = User.FindFirstValue(
                    ClaimTypes.Role
                )
            });
        }


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

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Username
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role
                ),

                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()
                ),

                new Claim(
                    JwtRegisteredClaimNames.UniqueName,
                    user.Username
                ),

                new Claim(
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
                    expires: DateTime.UtcNow.AddHours(8),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}