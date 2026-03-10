using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.Shared.DTOs;

namespace MyCyberQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AiChatController : ControllerBase
    {
        private readonly IAiChatService _aiChatService;

        public AiChatController(IAiChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        [HttpPost("ask")]
        public async Task<ActionResult<AiChatResponseDto>> AskAi([FromBody] AiChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserQuestion) || string.IsNullOrWhiteSpace(request.QuestionContext))
            {
                return BadRequest(new AiChatResponseDto { IsSuccessful = false, ErrorMessage = "Ogiltig data." });
            }

            var response = await _aiChatService.GetChatResponseAsync(request);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return StatusCode(500, response);
        }

    }
}
