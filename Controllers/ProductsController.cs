using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<Product> query = _context.Products.Include(p => p.User);

            if (userRole != "Admin" && userRole != "Manager")
            {
                query = query.Where(p => p.UserId == userId);
            }

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var product = await _context.Products
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            if (userRole != "Admin" && userRole != "Manager" && product.UserId != userId)
                return Forbid();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            product = await _context.Products
                .Include(p => p.User)
                .FirstAsync(p => p.Id == product.Id);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] ProductRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            if (userRole != "Admin" && userRole != "Manager" && product.UserId != userId)
                return Forbid();

            product.Name = request.Name;
            product.Price = request.Price;

            await _context.SaveChangesAsync();

            product = await _context.Products
                .Include(p => p.User)
                .FirstAsync(p => p.Id == product.Id);

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            if (userRole != "Admin" && product.UserId != userId)
                return Forbid();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
