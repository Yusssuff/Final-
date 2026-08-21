using Final_Task.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Filters;

namespace Final_Task.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
       
        private const string OperationName = "Products";

        private readonly AppDbContext _db;
        private readonly SalesBuzzPermissionService _permissions;

        public ProductsController(
            AppDbContext db,
            SalesBuzzPermissionService permissions)
        {
            _db = db;
            _permissions = permissions;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            if (!_permissions.HasPermission(
                    OperationName,
                    PermissionKind.Read))
            {
                return Forbid();
            }

            return Ok(_db.Products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (!_permissions.HasPermission(
                    OperationName,
                    PermissionKind.Read))
            {
                return Forbid();
            }

            var product =
                await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] Product product)
        {
            if (!_permissions.HasPermission(
                    OperationName,
                    PermissionKind.Create))
            {
                return Forbid();
            }

            if (product == null)
            {
                return BadRequest(new
                {
                    message = "Product is required."
                });
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest(new
                {
                    message = "Product name is required."
                });
            }

            var newProduct = new Product
            {
                Name = product.Name.Trim(),
                Price = product.Price,
                Quantity = product.Quantity
            };

            await _db.Products.AddAsync(newProduct);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new
                {
                    id = newProduct.Id
                },
                newProduct
            );
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromBody] Product product)
        {
            if (!_permissions.HasPermission(
                    OperationName,
                    PermissionKind.Update))
            {
                return Forbid();
            }

            if (product == null)
            {
                return BadRequest(new
                {
                    message = "Product is required."
                });
            }

            var existingProduct =
                await _db.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            existingProduct.Name =
                product.Name?.Trim() ??
                existingProduct.Name;

            existingProduct.Price =
                product.Price;

            existingProduct.Quantity =
                product.Quantity;

            await _db.SaveChangesAsync();

            return Ok(existingProduct);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!_permissions.HasPermission(
                    OperationName,
                    PermissionKind.Delete))
            {
                return Forbid();
            }

            var existingProduct =
                await _db.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            _db.Products.Remove(existingProduct);

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Product deleted successfully.",
                product = existingProduct
            });
        }
    }
}