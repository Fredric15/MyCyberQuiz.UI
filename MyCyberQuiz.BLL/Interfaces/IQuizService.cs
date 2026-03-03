using MyCyberQuiz.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.BLL.Interfaces
{
    public interface IQuizService
    {
        // En metod för att hämta all data vi behöver till menyn
        Task<IEnumerable<CategoryDto>> GetMenuCategoriesAsync();
        Task<QuizDetailsDto> GetQuizByIdAsync(int subCategoryId);
        Task<QuizResultDto> SubmitQuizAsync(SubmitQuizDto submission);

        Task<AnswerFeedbackDto> CheckAnswerAsync(int questionId, int selectedOptionId);
    }
}
