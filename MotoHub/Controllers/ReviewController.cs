
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoHub.DTOs;
using MotoHub.Services;
using System.Security.Claims;

namespace MotoHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _service;

        public ReviewController(ReviewService service)
        {
            _service = service;
        }

        private int CurrentUserId
        {
            get
            {
                var id = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                return int.TryParse(id, out var userId)
                    ? userId
                    : 0;
            }
        }

        // Переглянути відгуки про конкретний мотоцикл
        [HttpGet("motorcycle/{motorcycleId:int}")]
        public async Task<IActionResult> GetByMotorcycle(
            int motorcycleId)
        {
            return Ok(
                await _service.GetByMotorcycleAsync(motorcycleId));
        }

        // Залишити відгук може авторизований користувач
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateReviewDto dto)
        {
            if (CurrentUserId <= 0)
                return Unauthorized();

            var review = await _service.CreateAsync(
                CurrentUserId, dto);

            if (review == null)
            {
                return BadRequest(
                    "Перевірте мотоцикл, оцінку та коментар. " +
                    "Можливо, ви вже залишили відгук.");
            }

            return Ok(review);
        }

        // Власник може видалити свій відгук,
        // адміністратор — будь-який
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (CurrentUserId <= 0)
                return Unauthorized();

            var deleted = await _service.DeleteAsync(
                id,
                CurrentUserId,
                User.IsInRole("admin"));

            if (!deleted)
            {
                return NotFound(
                    "Відгук не знайдений або у вас немає права його видаляти.");
            }

            return NoContent();
        }
    }
}
