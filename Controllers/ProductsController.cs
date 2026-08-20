using Final_Task.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Final_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(_db.Products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }



        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
             Product p = new ()
            {
                 
                 Name =  product.Name,
                Price = product.Price,
                Quantity = product.Quantity
            };
            await _db.Products.AddAsync(p);
            await _db.SaveChangesAsync();
            return Ok(p);

        }
    }
}
