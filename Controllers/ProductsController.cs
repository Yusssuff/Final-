using Final_Task.Data;
using Final_Task.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Data;

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
        private readonly ICurrentBUContext _currentBUContext;

        public ProductsController(
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
        // GET ALL PRODUCTS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            if (!_permissions.HasPermission(
                OperationName,
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

            var products =
                await _db.Products
                    .ToListAsync();

            return Ok(products);
        }

        // =========================================================
        // GET PRODUCT BY ID
        // =========================================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(
            int id)
        {
            if (!_permissions.HasPermission(
                OperationName,
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

            var product =
                await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message =
                        "Product not found."
                });
            }

            return Ok(product);
        }

        // =========================================================
        // CREATE PRODUCT
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] Product product)
        {
            if (!_permissions.HasPermission(
                OperationName,
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

            if (product == null)
            {
                return BadRequest(new
                {
                    message =
                        "Product is required."
                });
            }

            if (string.IsNullOrWhiteSpace(
                product.Name))
            {
                return BadRequest(new
                {
                    message =
                        "Product name is required."
                });
            }

            if (product.Price < 0)
            {
                return BadRequest(new
                {
                    message =
                        "Price cannot be negative."
                });
            }

            if (product.Quantity < 0)
            {
                return BadRequest(new
                {
                    message =
                        "Quantity cannot be negative."
                });
            }

            var newProduct = new Product
            {
                Name =
                    product.Name.Trim(),

                Price =
                    product.Price,

                Quantity =
                    product.Quantity
            };

            await _db.Products.AddAsync(
                newProduct);

            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new
                {
                    id = newProduct.Id
                },
                newProduct);
        }

        // =========================================================
        // UPDATE PRODUCT
        // =========================================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromBody] Product product)
        {
            if (!_permissions.HasPermission(
                OperationName,
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

            if (product == null)
            {
                return BadRequest(new
                {
                    message =
                        "Product is required."
                });
            }

            if (string.IsNullOrWhiteSpace(
                product.Name))
            {
                return BadRequest(new
                {
                    message =
                        "Product name is required."
                });
            }

            if (product.Price < 0)
            {
                return BadRequest(new
                {
                    message =
                        "Price cannot be negative."
                });
            }

            if (product.Quantity < 0)
            {
                return BadRequest(new
                {
                    message =
                        "Quantity cannot be negative."
                });
            }

            var existingProduct =
                await _db.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new
                {
                    message =
                        "Product not found."
                });
            }

            existingProduct.Name =
                product.Name.Trim();

            existingProduct.Price =
                product.Price;

            existingProduct.Quantity =
                product.Quantity;

            await _db.SaveChangesAsync();

            return Ok(existingProduct);
        }

        // =========================================================
        // DELETE PRODUCT
        // =========================================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(
            int id)
        {
            if (!_permissions.HasPermission(
                OperationName,
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

            var existingProduct =
                await _db.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new
                {
                    message =
                        "Product not found."
                });
            }

            _db.Products.Remove(
                existingProduct);

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Product deleted successfully.",

                product =
                    existingProduct
            });
        }
    }
}