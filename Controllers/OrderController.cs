using Final_Task.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Filters;

namespace Final_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly SalesBuzzPermissionService _permissions;

        public OrderController(AppDbContext db, SalesBuzzPermissionService permissions)
        {
            _db = db;
            _permissions = permissions;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            if (!_permissions.HasPermission("Orders", PermissionKind.Read))
            {
                return Forbid();
            }

            return Ok(_db.Orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            if (!_permissions.HasPermission("Orders", PermissionKind.Create))
            {
                return Forbid();
            }

            // If the client did not supply a user id, take it from the token
            if (request.UserId == 0 && User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var uid))
                {
                    request.UserId = uid;
                }
            }

            var product = await _db.Products.FindAsync(request.ProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            Order order = new()
            {
                UserId = request.UserId,
                ProductId = product.Id,
                Quantity = request.Quantity,
                TotalPrice = product.Price * request.Quantity,
                OrderDate = DateTime.UtcNow,
                Product = product
            };
            product.Quantity -= request.Quantity;

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            order.Product = product;

            return Ok(order);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            if (!_permissions.HasPermission("Orders", PermissionKind.Read))
            {
                return Forbid();
            }

            var order = await _db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id,CreateOrderRequest request)
        {
            if (!_permissions.HasPermission("Orders", PermissionKind.Update))
            {
                return Forbid();
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var existingOrder = await _db.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound("Order not found.");
            }

            var newProduct = await _db.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (newProduct == null)
            {
                return NotFound("Product not found.");
            }

            if (existingOrder.Product != null)
            {
                existingOrder.Product.Quantity += existingOrder.Quantity;
            }

            if (request.Quantity > newProduct.Quantity)
            {
                return BadRequest(
                    $"Not enough stock. Available quantity: {newProduct.Quantity}.");
            }

            existingOrder.UserId = request.UserId;
            existingOrder.ProductId = newProduct.Id;
            existingOrder.Quantity = request.Quantity;
            existingOrder.TotalPrice = newProduct.Price * request.Quantity;
            existingOrder.OrderDate = DateTime.UtcNow;

            newProduct.Quantity -= request.Quantity;

            await _db.SaveChangesAsync();

            existingOrder.Product = newProduct;

            return Ok(existingOrder);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (!_permissions.HasPermission("Orders", PermissionKind.Delete))
            {
                return Forbid();
            }

            var existingOrder = await _db.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound("Order not found.");
            }

            if (existingOrder.Product != null)
            {
                existingOrder.Product.Quantity += existingOrder.Quantity;
            }

            _db.Orders.Remove(existingOrder);

            await _db.SaveChangesAsync();

            return Ok(existingOrder);
        }




    }
}
