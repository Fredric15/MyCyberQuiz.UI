using MyCyberQuiz.Shared.DTOs;
namespace MyCyberQuiz.UI.Services.Interfaces
{
    public interface IFrontendQuizService
    {
        Task<List<CategoryDto>> GetQuizMenuAsync();
        Task<QuizDetailsDto> GetQuizByIdAsync(int id);
        Task<QuizResultDto> SubmitQuizAsync(SubmitQuizDto submission);
        Task<AnswerFeedbackDto> CheckSingleAnswerAsync(int questionId, int selectedOptionId);
    }
}
