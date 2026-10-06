using Microsoft.AspNetCore.Mvc;
using MotoHub.DTOs;
using MotoHub.Services;

namespace MotoHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await _authService.Register(dto);

            if (token == null)
                return Conflict("Користувач з таким Email вже існує.");

            return Ok(new
            {
                token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto);

            if (token == null)
                return Unauthorized("Неправильний Email або пароль.");

            return Ok(new
            {
                token
            });
        }
    }
}