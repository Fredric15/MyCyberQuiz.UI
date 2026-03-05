using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{

    public class UserProfileDto
    {
        public string Email { get; set; } = string.Empty;

        // --- GENERELL PROGRESSION ---
        public int CompletedSubCategories { get; set; }
        public int TotalSubCategories { get; set; }

        // Räknar automatiskt ut hur många procent användaren har klarat!
        public int ProgressPercentage => TotalSubCategories == 0 ? 0 :
            (int)Math.Round((double)CompletedSubCategories / TotalSubCategories * 100);

        // --- STATISTIK ---
        public int TotalQuizzesPlayed { get; set; }
        public int TotalCorrectAnswers { get; set; }

        // --- HISTORIK ---
        public List<QuizHistoryDto> RecentHistory { get; set; } = new();
    }

    public class QuizHistoryDto
    {
        public string QuizName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CompletedAt { get; set; }
        public bool Passed { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Nuvarande lösenord krävs.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nytt lösenord krävs.")]
        [MinLength(6, ErrorMessage = "Lösenordet måste vara minst 6 tecken.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bekräfta det nya lösenordet.")]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class ChangeEmailDto
    {
        [Required(ErrorMessage = "Ny e-postadress krävs.")]
        [EmailAddress(ErrorMessage = "Ogiltig e-postadress.")]
        public string NewEmail { get; set; } = string.Empty;
    }
}
