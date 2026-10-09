
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoHub.DTOs;
using MotoHub.Services;

namespace MotoHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly MotoServiceService _service;

        public ServiceController(MotoServiceService service)
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
            var service = await _service.GetByIdAsync(id);

            if (service == null)
                return NotFound("Послугу не знайдено.");

            return Ok(service);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceDto dto)
        {
            var service = await _service.CreateAsync(dto);

            if (service == null)
                return BadRequest("Перевірте дані послуги.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = service.Id },
                service);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            CreateServiceDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return BadRequest(
                    "Послугу не знайдено або дані некоректні.");

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Послугу не знайдено.");

            return NoContent();
        }
    }
}
