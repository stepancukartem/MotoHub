using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoHub.DTOs;
using MotoHub.Services;

namespace MotoHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly BrandService _service;

        public BrandController(BrandService service)
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
            var brand = await _service.GetByIdAsync(id);

            if (brand == null)
                return NotFound("Бренд не знайдений.");

            return Ok(brand);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandDto dto)
        {
            var brand = await _service.CreateAsync(dto);

            if (brand == null)
                return BadRequest("Назва порожня або такий бренд уже існує.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = brand.Id },
                brand);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            CreateBrandDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return BadRequest(
                    "Бренд не знайдений або назва вже використовується.");

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return BadRequest(
                    "Бренд не знайдений або використовується мотоциклами.");

            return NoContent();
        }
    }
}
