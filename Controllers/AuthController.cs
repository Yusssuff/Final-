using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Final_Task.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext db,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // ============================================================
        // GET ROLES
        // ============================================================

        [AllowAnonymous]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var connectionString = GetConnectionString();

            var roles = new List<RoleOption>();

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            await using var command =
                connection.CreateCommand();

            command.CommandText = """
                SELECT
                    RoleID,
                    COALESCE(
                        NULLIF(Description, ''),
                        RoleID
                    ) AS Description
                FROM dbo.HH_SA_Roles
                ORDER BY Description;
                """;

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var roleId =
                    reader.IsDBNull(0)
                        ? string.Empty
                        : reader.GetString(0).Trim();

                var description =
                    reader.IsDBNull(1)
                        ? roleId
                        : reader.GetString(1).Trim();

                roles.Add(
                    new RoleOption
                    {
                        RoleID = roleId,
                        Description = description
                    });
            }

            return Ok(roles);
        }

        // ============================================================
        // REGISTER
        // ============================================================

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Request body is required."
                });
            }

            var username =
                request.Username?.Trim();

            var requestedRole =
                request.Role?.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new
                {
                    message = "Username is required."
                });
            }

            if (string.IsNullOrWhiteSpace(
                request.Password))
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

            if (string.IsNullOrWhiteSpace(
                requestedRole))
            {
                return BadRequest(new
                {
                    message = "Role is required."
                });
            }

            // --------------------------------------------------------
            // Check username
            // --------------------------------------------------------

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

            // --------------------------------------------------------
            // Find role in SalesBuzz
            // --------------------------------------------------------

            var role =
                await FindRoleAsync(
                    requestedRole);

            if (role == null)
            {
                return BadRequest(new
                {
                    message =
                        "Selected role does not exist."
                });
            }

            // --------------------------------------------------------
            // Create application user
            // --------------------------------------------------------

            var user = new User
            {
                Username = username,
                Role = role.RoleID
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.Password);

            await _db.Users.AddAsync(user);

            await _db.SaveChangesAsync();



            var defaultBuid =
                await GetDefaultBuidAsync();

            if (!string.IsNullOrWhiteSpace(
                defaultBuid))
            {
                await AssignUserToBuidAsync(
                    user.Id,
                    defaultBuid);
            }

            return Ok(new
            {
                message =
                    "Registration successful.",

                user = new
                {
                    id = user.Id,

                    username =
                        user.Username,

                    role =
                        role.Description,

                    roleId =
                        role.RoleID,

                    buid =
                        defaultBuid
                }
            });
        }

        // ============================================================
        // LOGIN
        // ============================================================

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

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(
                    request.Password))
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid username or password."
                });
            }

            // --------------------------------------------------------
            // Find user
            // --------------------------------------------------------

            var user =
                await _db.Users
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

            // --------------------------------------------------------
            // Verify password
            // --------------------------------------------------------

            var passwordResult =
                _passwordHasher
                    .VerifyHashedPassword(
                        user,
                        user.PasswordHash,
                        request.Password);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid username or password."
                });
            }

            // --------------------------------------------------------
            // Validate role
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                user.Role))
            {
                return Unauthorized(new
                {
                    message =
                        "User role is not configured."
                });
            }

            var role =
                await FindRoleAsync(
                    user.Role);

            if (role == null)
            {
                return Unauthorized(new
                {
                    message =
                        "User role does not exist."
                });
            }

            // --------------------------------------------------------
            // Get user's BUID
            //---------------------------------------------------------
            
            var buid =
                await GetUserBuidAsync(
                    user.Id)
                ?? await GetDefaultBuidAsync();

            if (!string.IsNullOrWhiteSpace(buid))
            {
                await AssignUserToBuidAsync(
                    user.Id,
                    buid);
            }

            // --------------------------------------------------------
            // Generate JWT
            // --------------------------------------------------------

            var token =
                GenerateToken(
                    user,
                    role.RoleID,
                    buid);

            return Ok(new
            {
                message =
                    "Login successful.",

                token,

                tokenType =
                    "Bearer",

                expiresIn =
                    15 * 60,

                user = new
                {
                    id =
                        user.Id,

                    username =
                        user.Username,

                    role =
                        role.Description,

                    roleId =
                        role.RoleID,

                    buid
                }
            });
        }

        // ============================================================
        // CURRENT USER
        // ============================================================

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
                        ClaimTypes.Role),

                buid =
                    User.FindFirstValue(
                        "BUID")
            });
        }

        // ============================================================
        // CHANGE PASSWORD
        // ============================================================

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return BadRequest(new { message = "All password fields are required." });
            }

            if (request.NewPassword.Length < 6)
            {
                return BadRequest(new { message = "New password must be at least 6 characters." });
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "New password and confirmation do not match." });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "User identity is invalid." });
            }

            var user = await _db.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return BadRequest(new { message = "Current password is incorrect." });
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }

        // ============================================================
        // FIND SALESBUZZ ROLE
        // ============================================================

        private async Task<RoleOption?> FindRoleAsync(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var connectionString =
                GetConnectionString();

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            await using var command =
                connection.CreateCommand();

            command.CommandText = """
                SELECT TOP 1
                    RoleID,
                    COALESCE(
                        NULLIF(Description, ''),
                        RoleID
                    ) AS Description
                FROM dbo.HH_SA_Roles
                WHERE
                    LOWER(RoleID) =
                        LOWER(@value)
                    OR LOWER(Description) =
                        LOWER(@value)
                    OR LOWER(DescriptionA) =
                        LOWER(@value);
                """;

            command.Parameters.Add(
                new SqlParameter(
                    "@value",
                    value.Trim()));

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var roleId =
                reader.IsDBNull(0)
                    ? string.Empty
                    : reader.GetString(0).Trim();

            var description =
                reader.IsDBNull(1)
                    ? roleId
                    : reader.GetString(1).Trim();

            return new RoleOption
            {
                RoleID = roleId,
                Description = description
            };
        }

        // ============================================================
        // GET DEFAULT BUID
        // ============================================================

        private async Task<string?> GetDefaultBuidAsync()
        {
            var connectionString =
                GetConnectionString();

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            await using var command =
                connection.CreateCommand();

            command.CommandText = """
                SELECT TOP 1
                    BUID
                FROM dbo.HH_SA_BU
                ORDER BY BUID;
                """;

            var result =
                await command.ExecuteScalarAsync();

            if (result == null ||
                result == DBNull.Value)
            {
                return null;
            }

            var buid =
                result.ToString()?.Trim();

            return string.IsNullOrWhiteSpace(buid)
                ? null
                : buid;
        }

        // ============================================================
        // GET USER BUID
        // ============================================================

        private async Task<string?> GetUserBuidAsync(
            int userId)
        {
            var connectionString =
                GetConnectionString();

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            await using var command =
                connection.CreateCommand();

            command.CommandText = """
                SELECT TOP 1
                    BUID
                FROM dbo.HH_SA_UserBUPermissions
                WHERE UserID = @userId
                ORDER BY BUID;
                """;

            command.Parameters.Add(
                new SqlParameter(
                    "@userId",
                    userId.ToString()));

            var result =
                await command.ExecuteScalarAsync();

            if (result == null ||
                result == DBNull.Value)
            {
                return null;
            }

            var buid =
                result.ToString()?.Trim();

            return string.IsNullOrWhiteSpace(buid)
                ? null
                : buid;
        }

        // ============================================================
        // ASSIGN USER TO BUID
        // ============================================================

        private async Task AssignUserToBuidAsync(
            int userId,
            string buid)
        {
            if (string.IsNullOrWhiteSpace(buid))
            {
                return;
            }

            var connectionString =
                GetConnectionString();

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            // Check whether assignment already exists.

            await using (
                var checkCommand =
                    connection.CreateCommand())
            {
                checkCommand.CommandText = """
                    SELECT COUNT(1)
                    FROM dbo.HH_SA_UserBUPermissions
                    WHERE UserID = @userId
                      AND BUID = @buid;
                    """;

                checkCommand.Parameters.Add(
                    new SqlParameter(
                        "@userId",
                        userId.ToString()));

                checkCommand.Parameters.Add(
                    new SqlParameter(
                        "@buid",
                        buid.Trim()));

                var exists =
                    Convert.ToInt32(
                        await checkCommand
                            .ExecuteScalarAsync());

                if (exists > 0)
                {
                    return;
                }
            }

            await using var insertCommand =
                connection.CreateCommand();

            insertCommand.CommandText = """
                INSERT INTO dbo.HH_SA_UserBUPermissions
                (
                    UserID,
                    BUID,
                    CreatedOn,
                    Createdby
                )
                VALUES
                (
                    @userId,
                    @buid,
                    SYSDATETIME(),
                    @createdBy
                );
                """;

            insertCommand.Parameters.Add(
                new SqlParameter(
                    "@userId",
                    userId.ToString()));

            insertCommand.Parameters.Add(
                new SqlParameter(
                    "@buid",
                    buid.Trim()));

            insertCommand.Parameters.Add(
                new SqlParameter(
                    "@createdBy",
                    "system"));

            await insertCommand.ExecuteNonQueryAsync();
        }

        // ============================================================
        // GENERATE JWT
        // ============================================================

        private string GenerateToken(
            User user,
            string roleId,
            string? buid)
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
                    "JWT:Key is missing.");
            }

            if (string.IsNullOrWhiteSpace(
                issuer))
            {
                throw new InvalidOperationException(
                    "JWT:ValidIssuer is missing.");
            }

            if (string.IsNullOrWhiteSpace(
                audience))
            {
                throw new InvalidOperationException(
                    "JWT:ValidAudience is missing.");
            }

            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        ClaimTypes.Name,
                        user.Username),

                    // SalesBuzz reads this claim
                    // to determine the user's role.
                    new Claim(
                        ClaimTypes.Role,
                        roleId),

                    // Application role claim.
                    new Claim(
                        "role_id",
                        roleId),

                    new Claim(
                        JwtRegisteredClaimNames.Sub,
                        user.Id.ToString()),

                    new Claim(
                        JwtRegisteredClaimNames.UniqueName,
                        user.Username),

                    new Claim(
                        JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                };

            if (!string.IsNullOrWhiteSpace(buid))
            {
                claims.Add(
                    new Claim(
                        "BUID",
                        buid.Trim()));
            }

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    notBefore:
                        DateTime.UtcNow,
                    expires:
                        DateTime.UtcNow.AddMinutes(15),
                    signingCredentials:
                        credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        // ============================================================
        // CONNECTION STRING
        // ============================================================

        private string GetConnectionString()
        {
            var connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            if (string.IsNullOrWhiteSpace(
                connectionString))
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection is missing.");
            }

            return connectionString;
        }
    }

    // ================================================================
    // ROLE DTO
    // ================================================================

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ================================================================
    // ROLE DTO
    // ================================================================
    public class RoleOption
    {
        public string RoleID { get; set; } =
            string.Empty;

        public string Description { get; set; } =
            string.Empty;
    }
}