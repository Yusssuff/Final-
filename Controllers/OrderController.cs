using Final_Task.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Final_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrderController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
           
            return Ok(_db.Orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
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

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            return Ok(order);
        }




    }
}
