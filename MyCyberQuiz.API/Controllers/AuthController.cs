using Microsoft.AspNetCore.Mvc;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.Shared.DTOs;

namespace MyCyberQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // 1. Skicka vidare uppgifterna till BLL
            var result = await _authService.LoginAsync(loginDto);

            // 2. Om inloggningen lyckades, returnera 200 OK med token-svaret
            if (result.IsSuccessful)
            {
                return Ok(result);
            }

            // 3. Om det misslyckades (fel lösenord/epost), returnera 401 Unauthorized
            return Unauthorized(result);
        }
    }
}
