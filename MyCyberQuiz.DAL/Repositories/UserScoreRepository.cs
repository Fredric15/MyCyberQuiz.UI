using Microsoft.EntityFrameworkCore;
using MyCyberQuiz.DAL.Data;
using MyCyberQuiz.DAL.Models;
using MyCyberQuiz.DAL.Repositories.Interfaces;

namespace MyCyberQuiz.DAL.Repositories
{
    public class UserScoreRepository : IUserScoreRepository
    {
        private readonly AppDbContext _context;
        public UserScoreRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddScoreAsync(UserScoreModel userScore)
        {
            // Lägger till ett nytt UserScore-objekt kopplat till en användare och ett quiz och sparar det i databasen
            await _context.UserScores.AddAsync(userScore);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserScoreModel>> GetUserScoreByUserIdAsync(string userId)
        {
            // Hämtar alla UserScore-objekt som matchar det angivna userId och returnerar dem som en lista
            // Detta används för att visa användarens poäng i olika quiz
            return await _context.UserScores
                .Include(us => us.Quiz) // Inkluderar relaterad Quiz-information för att kunna visa quiznamn eller andra detaljer
                .Where(us => us.ApplicationUserId == userId)
                .ToListAsync();
        }
    }
}
