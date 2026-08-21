using Microsoft.AspNetCore.Mvc;
using Final_Task.Data;
using AuthMethod; 
using SalesBuzz.Shared.Authorization;

namespace Final_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Injecting the SDK's native authentication logic
        private readonly SalesBuzzAuth _salesBuzzAuth;
        private readonly IPermissions _permissions;

        public AuthController(
            SalesBuzzAuth salesBuzzAuth,
            IPermissions permissions)
        {
            _salesBuzzAuth = salesBuzzAuth;
            _permissions = permissions;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // NOTE: Type "_salesBuzzAuth." in Visual Studio to let IntelliSense 
                // show you the exact method name. I am using "LoginAsync" as an assumption.

                var loginResponse = await _salesBuzzAuth.LoginAsync(request.Username, request.Password);

                // If the SDK returns null/false on failure
                if (loginResponse == null)
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                // If the SDK requires you to manually trigger permission updates after auth
                // _permissions.UpdateUserPermissions(loginResponse.Role);

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                // The SDK likely throws an exception if the user is not found or password is bad
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // NOTE: Again, verify if the SDK actually handles registration. 
                // Some enterprise SDKs only handle Login, expecting users to be created via an Admin portal.

                var result = await _salesBuzzAuth.RegisterAsync(request.Username, request.Password);

                return Ok(new { message = "User registered successfully via SalesBuzz SDK." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}