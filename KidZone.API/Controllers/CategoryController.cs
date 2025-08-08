using KidZone.API.Data;
using KidZone.API.DTOs;
using KidZone.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.ContentCategories)
                    .ThenInclude(cc => cc.Content)
                .Select(c => new CategoryDto
                {
                    Id=c.CategoryID,
                    Name = c.Name,
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ContentCategories)
                    .ThenInclude(cc => cc.Content)
                .Where(c => c.CategoryID == id)
                .Select(c => new CategoryDto
                {
                    Id=c.CategoryID,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

              var result = new CategoryDto
            {
                Id = category.CategoryID,
                Name = category.Name,

            };

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            category.Name = dto.Name;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


}

