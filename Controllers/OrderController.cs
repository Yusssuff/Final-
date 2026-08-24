using Final_Task.Data;
using Final_Task.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Filters;

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

        public OrderController(
            AppDbContext db,
            SalesBuzzPermissionService permissions)
        {
            _db = db;
            _permissions = permissions;
        }

        // =========================================
        // GET ALL ORDERS
        // =========================================

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            if (!await _permissions.HasPermission(
                "Orders",
                PermissionKind.Read))
            {
                return Forbid();
            }

            var orders = await _db.Orders
                .Include(o => o.Product)
                .ToListAsync();

            return Ok(orders);
        }

        // =========================================
        // CREATE ORDER
        // =========================================

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderRequest request)
        {
            if (!await _permissions.HasPermission(
                "Orders",
                PermissionKind.Create))
            {
                return Forbid();
            }

            if (request == null)
            {
                return BadRequest(
                    "Order data is required.");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest(
                    "Quantity must be greater than zero.");
            }

            // Get authenticated user ID from JWT.
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                userIdClaim,
                out var userId))
            {
                return Unauthorized();
            }

            var product =
                await _db.Products.FindAsync(
                    request.ProductId);

            if (product == null)
            {
                return NotFound(
                    "Product not found.");
            }

            if (request.Quantity > product.Quantity)
            {
                return BadRequest(
                    $"Not enough stock. Available quantity: {product.Quantity}.");
            }

            var order = new Order
            {
                UserId = userId,
                ProductId = product.Id,
                Quantity = request.Quantity,
                TotalPrice =
                    product.Price * request.Quantity,
                OrderDate = DateTime.UtcNow,
                Product = product
            };

            product.Quantity -= request.Quantity;

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            return Ok(order);
        }

        // =========================================
        // GET ORDER BY ID
        // =========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            if (!await _permissions.HasPermission(
                "Orders",
                PermissionKind.Read))
            {
                return Forbid();
            }

            var order =
                await _db.Orders
                    .Include(o => o.Product)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (order == null)
            {
                return NotFound(
                    "Order not found.");
            }

            return Ok(order);
        }

        // =========================================
        // UPDATE ORDER
        // =========================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(
            int id,
            [FromBody] CreateOrderRequest request)
        {
            if (!await _permissions.HasPermission(
                "Orders",
                PermissionKind.Update))
            {
                return Forbid();
            }

            if (request == null)
            {
                return BadRequest(
                    "Order data is required.");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest(
                    "Quantity must be greater than zero.");
            }

            var existingOrder =
                await _db.Orders
                    .Include(o => o.Product)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound(
                    "Order not found.");
            }

            var newProduct =
                await _db.Products
                    .FirstOrDefaultAsync(
                        p => p.Id == request.ProductId);

            if (newProduct == null)
            {
                return NotFound(
                    "Product not found.");
            }

            // Return old quantity to stock.
            if (existingOrder.Product != null)
            {
                existingOrder.Product.Quantity +=
                    existingOrder.Quantity;
            }

            if (request.Quantity > newProduct.Quantity)
            {
                return BadRequest(
                    $"Not enough stock. Available quantity: {newProduct.Quantity}.");
            }

            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                userIdClaim,
                out var userId))
            {
                return Unauthorized();
            }

            existingOrder.UserId = userId;

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

        // =========================================
        // DELETE ORDER
        // =========================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (!await _permissions.HasPermission(
                "Orders",
                PermissionKind.Delete))
            {
                return Forbid();
            }

            var existingOrder =
                await _db.Orders
                    .Include(o => o.Product)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound(
                    "Order not found.");
            }

            // Return ordered quantity to stock.
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