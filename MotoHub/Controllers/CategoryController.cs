
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoHub.DTOs;
using MotoHub.Services;

namespace MotoHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetByIdAsync(id);

            if (category == null)
                return NotFound("Категорія не знайдена.");

            return Ok(category);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = await _service.CreateAsync(dto);

            if (category == null)
                return BadRequest(
                    "Назва порожня або така категорія вже існує.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                category);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            CreateCategoryDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return BadRequest(
                    "Категорія не знайдена або назва вже використовується.");

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return BadRequest(
                    "Категорія не знайдена або використовується мотоциклами.");

            return NoContent();
        }
    }
}
