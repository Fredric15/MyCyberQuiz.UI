using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.Shared.DTOs;
using System.Security.Claims;

namespace MyCyberQuiz.API.Controllers
{
    [Authorize] // Kräver att användaren är inloggad för att komma åt dessa endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        // DI: Controllern ber om en IQuizService när den skapas
        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("menu")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetMenuCategories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var menuData = await _quizService.GetMenuCategoriesAsync(userId);   
            return Ok(menuData);
        }

        // I QuizController.cs

        [HttpGet("{subCategoryId}")]
        public async Task<ActionResult<QuizDetailsDto>> GetQuiz(int subCategoryId)
        {
            var quiz = await _quizService.GetQuizByIdAsync(subCategoryId);

            if (quiz == null)
            {
                return NotFound($"Kunde inte hitta något quiz för subkategori med ID {subCategoryId}");
            }

            return quiz;
        }

        [HttpGet("question/{questionId}/check/{selectedOptionId}")]
        public async Task<ActionResult<AnswerFeedbackDto>> CheckSingleAnswer(int questionId, int selectedOptionId)
        {
            try
            {
                var feedback = await _quizService.CheckAnswerAsync(questionId, selectedOptionId);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz([FromBody] SubmitQuizDto submission)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Skickar DTO:n vidare till vår Service som gör hela rättningen
                var result = await _quizService.SubmitQuizAsync(submission, userId);

                return Ok(result); // Returnerar en 200 OK tillsammans med poängen
            }
            catch (Exception ex)
            {
                // Om något går fel (t.ex. att quizet inte fanns)
                return BadRequest(ex.Message);
            }
        }
    }
}
