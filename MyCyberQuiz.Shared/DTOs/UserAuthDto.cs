using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{
    // Skickas när en användare vill skapa ett konto
    public record RegisterDto(
        string Email,
        string Password,
        string ConfirmPassword
    );

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
