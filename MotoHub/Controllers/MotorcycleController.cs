
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoHub.DTOs;
using MotoHub.Services;

namespace MotoHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotorcycleController : ControllerBase
    {
        private readonly MotorcycleService _service;

        public MotorcycleController(MotorcycleService service)
        {
            _service = service;
        }

        // Перегляд усіх мотоциклів доступний усім
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        // Перегляд одного мотоцикла
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var motorcycle = await _service.GetByIdAsync(id);

            if (motorcycle == null)
                return NotFound("Мотоцикл не знайдений.");

            return Ok(motorcycle);
        }

        // Додавати мотоцикли може тільки admin
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateMotorcycleDto dto)
        {
            var motorcycle = await _service.CreateAsync(dto);

            if (motorcycle == null)
                return BadRequest(
                    "Перевірте BrandId та CategoryId.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = motorcycle.Id },
                motorcycle);
        }

        // Редагувати може тільки admin
        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateMotorcycleDto dto)
        {
            var motorcycle = await _service.UpdateAsync(id, dto);

            if (motorcycle == null)
                return BadRequest(
                    "Мотоцикл не знайдений або некоректні BrandId/CategoryId.");

            return Ok(motorcycle);
        }

        // Видаляти може тільки admin
        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Мотоцикл не знайдений.");

            return NoContent();
        }
    }
}
