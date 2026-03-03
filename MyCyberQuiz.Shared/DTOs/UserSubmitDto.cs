using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{
    // Huvudpaketet som skickas in när quizet är klart
    public record SubmitQuizDto(
        int QuizId,
        int SubCategoryId,
        List<UserAnswerDto> Answers
    );

    // Representerar ett enskilt svar på en specifik fråga
    public record UserAnswerDto(
        int QuestionId,
        int SelectedOptionId
    );

    // Skickas från servern tillbaka till Blazor efter rättning
    public record QuizResultDto(
        int QuizId,
        int TotalQuestions,
        int CorrectAnswers,
        bool Passed, // T.ex. true om man fick över 80%
        string Message,
        string? AiFeedback = null // Redo för din framtida AI-integration!
    );
}
