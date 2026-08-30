using Final_Task.Data;
using Final_Task.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Data;

using System.Security.Claims;

namespace Final_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly SalesBuzzPermissionService _permissions;
        private readonly ICurrentBUContext _currentBUContext;

        public OrderController(
            AppDbContext db,
            SalesBuzzPermissionService permissions,
            ICurrentBUContext currentBUContext)
        {
            _db = db;
            _permissions = permissions;
            _currentBUContext = currentBUContext;
        }

        private IActionResult? ValidateCurrentBuidAccess()
        {
            var currentBuid =
                _currentBUContext.GetUserBUID();

            var tokenBuid =
                User.FindFirst("BUID")?.Value;

            if (string.IsNullOrWhiteSpace(currentBuid) &&
                string.IsNullOrWhiteSpace(tokenBuid))
            {
                return Forbid();
            }

            var effectiveCurrentBuid =
                string.IsNullOrWhiteSpace(currentBuid)
                    ? tokenBuid
                    : currentBuid;

            var effectiveTokenBuid =
                string.IsNullOrWhiteSpace(tokenBuid)
                    ? currentBuid
                    : tokenBuid;

            if (string.IsNullOrWhiteSpace(effectiveCurrentBuid) ||
                string.IsNullOrWhiteSpace(effectiveTokenBuid))
            {
                return Forbid();
            }

            if (!string.Equals(
                    effectiveCurrentBuid.Trim(),
                    effectiveTokenBuid.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            return null;
        }

        // =========================================================
        // GET ALL ORDERS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            if (!_permissions.HasPermission(
                "Orders",
                SalesBuzz.Shared.Filters.PermissionKind.Read))
            {
                return Forbid();
            }

            var buAccessResult =
                ValidateCurrentBuidAccess();

            if (buAccessResult != null)
            {
                return buAccessResult;
            }

            var orders =
                await _db.Orders
                    .Include(o => o.Product)
                    .ToListAsync();

            return Ok(orders);
        }

                    // =========================================================
                    // GET MY ORDERS
                    // =========================================================

                    [HttpGet("my")]
                    public async Task<IActionResult> GetMyOrders()
                    {
                        if (!_permissions.HasPermission(
                            "Orders",
                            SalesBuzz.Shared.Filters.PermissionKind.Read))
                        {
                            return Forbid();
                        }

                        var buAccessResult = ValidateCurrentBuidAccess();
                        if (buAccessResult != null) return buAccessResult;

                        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (!int.TryParse(userIdClaim, out var userId))
                        {
                            return Unauthorized(new { message = "User identity is invalid." });
                        }

                        var orders = await _db.Orders
                            .Include(o => o.Product)
                            .Where(o => o.UserId == userId)
                            .ToListAsync();

                        return Ok(orders);
                    }

        // =========================================================
        // CREATE ORDER
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderRequest request)
        {
            if (!_permissions.HasPermission(
                "Orders",
                SalesBuzz.Shared.Filters.PermissionKind.Create))
            {
                return Forbid();
            }

            var buAccessResult =
                ValidateCurrentBuidAccess();

            if (buAccessResult != null)
            {
                return buAccessResult;
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Order data is required."
                });
            }

            if (request.Quantity <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Quantity must be greater than zero."
                });
            }

            // Get the authenticated user's ID
            // directly from the JWT.

            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                userIdClaim,
                out var userId))
            {
                return Unauthorized(new
                {
                    message =
                        "User identity is invalid."
                });
            }

            var product =
                await _db.Products.FindAsync(
                    request.ProductId);

            if (product == null)
            {
                return NotFound(new
                {
                    message =
                        "Product not found."
                });
            }

            if (request.Quantity >
                product.Quantity)
            {
                return BadRequest(new
                {
                    message =
                        $"Not enough stock. Available quantity: {product.Quantity}."
                });
            }

            var order = new Order
            {
                UserId =
                    userId,

                ProductId =
                    product.Id,

                Quantity =
                    request.Quantity,

                TotalPrice =
                    product.Price *
                    request.Quantity,

                OrderDate =
                    DateTime.UtcNow,

                Product =
                    product
            };

            product.Quantity -=
                request.Quantity;

            await _db.Orders.AddAsync(order);

            await _db.SaveChangesAsync();

            return Ok(order);
        }

        // =========================================================
        // GET ORDER BY ID
        // =========================================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(
            int id)
        {
            if (!_permissions.HasPermission(
                "Orders",
                SalesBuzz.Shared.Filters.PermissionKind.Read))
            {
                return Forbid();
            }

            var buAccessResult =
                ValidateCurrentBuidAccess();

            if (buAccessResult != null)
            {
                return buAccessResult;
            }

            var order =
                await _db.Orders
                    .Include(o => o.Product)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (order == null)
            {
                return NotFound(new
                {
                    message =
                        "Order not found."
                });
            }

            return Ok(order);
        }



        // =========================================================
        // UPDATE ORDER
        // =========================================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(
            int id,
            [FromBody] CreateOrderRequest request)
        {
            if (!_permissions.HasPermission(
                "Orders",
                SalesBuzz.Shared.Filters.PermissionKind.Update))
            {
                return Forbid();
            }

            var buAccessResult =
                ValidateCurrentBuidAccess();

            if (buAccessResult != null)
            {
                return buAccessResult;
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    message =
                        "Order data is required."
                });
            }

            if (request.Quantity <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Quantity must be greater than zero."
                });
            }

            var existingOrder =
                await _db.Orders
                    .Include(o => o.Product)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound(new
                {
                    message =
                        "Order not found."
                });
            }

            var newProduct =
                await _db.Products
                    .FirstOrDefaultAsync(
                        p =>
                            p.Id ==
                            request.ProductId);

            if (newProduct == null)
            {
                return NotFound(new
                {
                    message =
                        "Product not found."
                });
            }

            // Return the previous quantity
            // to the old product's stock.

            if (existingOrder.Product != null)
            {
                existingOrder.Product.Quantity +=
                    existingOrder.Quantity;
            }

            if (request.Quantity >
                newProduct.Quantity)
            {
                return BadRequest(new
                {
                    message =
                        $"Not enough stock. Available quantity: {newProduct.Quantity}."
                });
            }

            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                userIdClaim,
                out var userId))
            {
                return Unauthorized(new
                {
                    message =
                        "User identity is invalid."
                });
            }

            existingOrder.UserId =
                userId;

            existingOrder.ProductId =
                newProduct.Id;

            existingOrder.Quantity =
                request.Quantity;

            existingOrder.TotalPrice =
                newProduct.Price *
                request.Quantity;

            existingOrder.OrderDate =
                DateTime.UtcNow;

            newProduct.Quantity -=
                request.Quantity;

            await _db.SaveChangesAsync();

            existingOrder.Product =
                newProduct;

            return Ok(existingOrder);
        }

        // =========================================================
        // DELETE ORDER
        // =========================================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(
            int id)
        {
            if (!_permissions.HasPermission(
                "Orders",
                SalesBuzz.Shared.Filters.PermissionKind.Delete))
            {
                return Forbid();
            }

            var buAccessResult =
                ValidateCurrentBuidAccess();

            if (buAccessResult != null)
            {
                return buAccessResult;
            }

            var existingOrder =
                await _db.Orders
                    .Include(o => o.Product)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound(new
                {
                    message =
                        "Order not found."
                });
            }

            // Return the ordered quantity
            // to product stock.

            if (existingOrder.Product != null)
            {
                existingOrder.Product.Quantity +=
                    existingOrder.Quantity;
            }

            _db.Orders.Remove(existingOrder);

            await _db.SaveChangesAsync();

            return Ok(existingOrder);
        }
    }
}