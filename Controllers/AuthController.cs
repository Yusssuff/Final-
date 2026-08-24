using System.Data;
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
            var roles = new List<RoleOption>();

            await using var connection =
                _db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

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
                roles.Add(
                    new RoleOption
                    {
                        RoleID =
                            reader.GetString(0).Trim(),

                        Description =
                            reader.GetString(1).Trim()
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

            if (string.IsNullOrWhiteSpace(requestedRole))
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
                    message = "Username already exists."
                });
            }

            // --------------------------------------------------------
            // Find the selected role in SalesBuzz
            // HH_SA_Roles
            // --------------------------------------------------------

            var role =
                await FindRoleAsync(requestedRole);

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
            //
            // IMPORTANT:
            // Users.Role stores the SalesBuzz RoleID.
            // Example:
            //
            // User -> user
            // Admin -> admin
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
                        role.Description
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
                string.IsNullOrWhiteSpace(request.Password))
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
                _passwordHasher.VerifyHashedPassword(
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
            // Validate user's SalesBuzz role
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                return Unauthorized(new
                {
                    message =
                        "User role is not configured."
                });
            }

            var role =
                await FindRoleAsync(user.Role);

            if (role == null)
            {
                return Unauthorized(new
                {
                    message =
                        "User role does not exist."
                });
            }

            // --------------------------------------------------------
            // Generate JWT
            //
            // IMPORTANT:
            // SalesBuzz CurrentBUClass.GetUserRoleID()
            // reads ClaimTypes.Role.
            //
            // Therefore this must contain:
            //
            // admin
            // or
            // user
            //
            // NOT the display name.
            // --------------------------------------------------------

            var buid = await GetUserBuidAsync(user.Username);
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
                    8 * 60 * 60,

                user = new
                {
                    id = user.Id,

                    username =
                        user.Username,

                    role =
                        role.Description,

                    roleId =
                        role.RoleID
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
                        ClaimTypes.Role)
            });
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

            await using var connection =
                _db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

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
                    LOWER(RoleID) = LOWER(@value)
                    OR LOWER(Description) = LOWER(@value)
                    OR LOWER(DescriptionA) = LOWER(@value);
                """;

            var parameter =
                command.CreateParameter();

            parameter.ParameterName =
                "@value";

            parameter.Value =
                value.Trim();

            command.Parameters.Add(parameter);

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new RoleOption
            {
                RoleID =
                    reader.GetString(0).Trim(),

                Description =
                    reader.IsDBNull(1)
                        ? reader.GetString(0).Trim()
                        : reader.GetString(1).Trim()
            };
        }

        // ============================================================
        // GENERATE JWT
        // ============================================================

        private string GenerateToken(
            User user,
                    string roleId,
                    string? buid = null)
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

                    var claims =
                        new List<Claim>
                        {
                            // User ID
                            new Claim(
                                ClaimTypes.NameIdentifier,
                                user.Id.ToString()),

                            // Username
                            new Claim(
                                ClaimTypes.Name,
                                user.Username),

                            // IMPORTANT:
                            // SalesBuzz reads this claim
                            // to determine the user's role.
                            new Claim(
                                ClaimTypes.Role,
                                roleId),

                            // Optional role_id claim for our app
                            new Claim(
                                "role_id",
                                roleId),

                            // JWT subject
                            new Claim(
                                JwtRegisteredClaimNames.Sub,
                                user.Id.ToString()),

                            // JWT username
                            new Claim(
                                JwtRegisteredClaimNames.UniqueName,
                                user.Username),

                            // JWT ID
                            new Claim(
                                JwtRegisteredClaimNames.Jti,
                                Guid.NewGuid().ToString())
                        };

                    if (!string.IsNullOrWhiteSpace(buid))
                    {
                        claims.Add(new Claim("BUID", buid!.Trim()));
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
                            notBefore: DateTime.UtcNow,
                            expires:
                                DateTime.UtcNow.AddHours(8),
                            signingCredentials:
                                credentials
                        );

                    return new JwtSecurityTokenHandler()
                        .WriteToken(token);
                }

                private async Task<string?> GetUserBuidAsync(string username)
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return null;
                    }

                    // Try to use the DbContext's connection first. Some SDK registrations
                    // may not configure the connection string on the context's underlying
                    // DbConnection, so fall back to creating a SqlConnection from
                    // configuration when necessary.
                    var dbConnection = _db.Database.GetDbConnection();
                    var useExplicitSqlConnection = string.IsNullOrWhiteSpace(dbConnection.ConnectionString);

                    if (!useExplicitSqlConnection)
                    {
                        // Use the DbContext's connection (do not dispose it here — DbContext owns it)
                        var connection = dbConnection;

                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }

                        await using var command = connection.CreateCommand();

                        command.CommandText = @"
                            SELECT TOP 1 BUID
                            FROM dbo.HH_SA_UserBUPermissions
                            WHERE LOWER(UserID) = LOWER(@user)
                            ";

                        var param = command.CreateParameter();
                        param.ParameterName = "@user";
                        param.Value = username.Trim();
                        command.Parameters.Add(param);

                        await using var reader = await command.ExecuteReaderAsync();

                        if (await reader.ReadAsync())
                        {
                            return reader.IsDBNull(0) ? null : reader.GetString(0).Trim();
                        }

                        // Fallback: pick a default BU from HH_SA_BU
                        reader.Dispose();
                        command.Parameters.Clear();

                        command.CommandText = @"
                            SELECT TOP 1 BUID
                            FROM dbo.HH_SA_BU
                            ORDER BY BUID
                            ";

                        await using var reader2 = await command.ExecuteReaderAsync();

                        if (await reader2.ReadAsync())
                        {
                            return reader2.IsDBNull(0) ? null : reader2.GetString(0).Trim();
                        }

                        return null;
                    }
                    else
                    {
                        var connStr = _configuration.GetConnectionString("DefaultConnection");

                        if (string.IsNullOrWhiteSpace(connStr))
                        {
                            // No connection string available to create a fallback connection.
                            return null;
                        }

                        await using var connection = new SqlConnection(connStr);
                        await connection.OpenAsync();

                        await using var command = connection.CreateCommand();

                        command.CommandText = @"
                            SELECT TOP 1 BUID
                            FROM dbo.HH_SA_UserBUPermissions
                            WHERE LOWER(UserID) = LOWER(@user)
                            ";

                        var param = command.CreateParameter();
                        param.ParameterName = "@user";
                        param.Value = username.Trim();
                        command.Parameters.Add(param);

                        await using var reader = await command.ExecuteReaderAsync();

                        if (await reader.ReadAsync())
                        {
                            return reader.IsDBNull(0) ? null : reader.GetString(0).Trim();
                        }

                        // Fallback: pick a default BU from HH_SA_BU
                        command.Parameters.Clear();
                        command.CommandText = @"
                            SELECT TOP 1 BUID
                            FROM dbo.HH_SA_BU
                            ORDER BY BUID
                            ";

                        await using var reader2 = await command.ExecuteReaderAsync();

                        if (await reader2.ReadAsync())
                        {
                            return reader2.IsDBNull(0) ? null : reader2.GetString(0).Trim();
                        }

                        return null;
                    }
                }
    }

    // ================================================================
    // ROLE DTO
    // ================================================================

    public sealed class RoleOption
    {
        public string RoleID { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}