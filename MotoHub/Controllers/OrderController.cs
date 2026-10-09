
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoHub.DTOs;
using MotoHub.Services;
using System.Security.Claims;

namespace MotoHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service;

        public OrderController(OrderService service)
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

        private bool IsAdmin =>
            User.IsInRole("admin");

        // Створити власне замовлення
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            if (CurrentUserId <= 0)
                return Unauthorized();

            var order = await _service.CreateAsync(
                CurrentUserId, dto);

            if (order == null)
                return BadRequest(
                    "Перевірте список товарів, їхні ID та кількість.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                order);
        }

        // Користувач бачить тільки свої замовлення
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            if (CurrentUserId <= 0)
                return Unauthorized();

            return Ok(
                await _service.GetUserOrdersAsync(CurrentUserId));
        }

        // Усі замовлення доступні тільки адміністратору
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        // Користувач бачить власне замовлення,
        // адміністратор може переглядати будь-яке
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (CurrentUserId <= 0)
                return Unauthorized();

            var order = await _service.GetByIdAsync(
                id, CurrentUserId, IsAdmin);

            if (order == null)
                return NotFound("Замовлення не знайдено.");

            return Ok(order);
        }

        // Змінювати статус може тільки адміністратор
        [Authorize(Roles = "admin")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            UpdateOrderStatusDto dto)
        {
            var updated = await _service.UpdateStatusAsync(
                id, dto.Status);

            if (!updated)
                return BadRequest(
                    "Замовлення не знайдено або статус некоректний.");

            return Ok(new
            {
                message = "Статус замовлення оновлено.",
                status = dto.Status
            });
        }
    }
}
