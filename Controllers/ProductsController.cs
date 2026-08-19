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
    }
}
