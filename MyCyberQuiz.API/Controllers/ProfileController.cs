using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.Shared.DTOs;
using System.Security.Claims;

namespace MyCyberQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IQuizService _quizService;

        public ProfileController(IAuthService authService, IQuizService quizService)
        {
            _authService = authService;
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kunde inte identifiera användaren.");
            }

            var profile = await _quizService.GetUserProfileAsync(userId);

            if (profile == null)
            {
                return NotFound("Användarprofilen kunde inte hittas i databasen.");
            }

            return Ok(profile);
        }

        
        [HttpPut("change-password")]
        public async Task<ActionResult<AuthResponseDto>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, dto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpPut("change-email")]
        public async Task<ActionResult<AuthResponseDto>> ChangeEmail([FromBody] ChangeEmailDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _authService.ChangeEmailAsync(userId, dto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

