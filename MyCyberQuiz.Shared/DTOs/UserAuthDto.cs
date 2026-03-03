using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{
    // Skickas när en användare vill skapa ett konto
    public record RegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Ogiltig e-postadress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Lösenord krävs")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte")]
        public string ConfirmPassword { get; set; }
    }

    // Skickas när en användare vill logga in
    public record LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }



    // Skickas tillbaka från API:et när inloggningen är klar (eller misslyckades)
    public record AuthResponseDto(
        bool IsSuccessful,
        string? ErrorMessage,
        string? Token // Här kommer vi skicka den hemliga "inloggnings-nyckeln" (JWT)
    );
}
