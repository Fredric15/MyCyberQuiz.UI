using MyCyberQuiz.DAL.Models;

namespace MyCyberQuiz.DAL.Repositories.Interfaces
{
    public interface IUserProgressRepository
    {
        // Hämtar alla UserProgress-objekt för en specifik användare
        Task<IEnumerable<UserProgressModel>> GetUserProgressByUserIdAsync(string userId);
        // Hämtar ett specifikt UserProgress-objekt för en användare och en subkategori
        Task<UserProgressModel?> GetProgressAsync(string userId, int subCategoryId);

        // Lägger till ett nytt UserProgress-objekt för en användare
        Task AddProgressAsync(UserProgressModel userProgress);
        
        // Uppdaterar ett befintligt UserProgress-objekt för en användare
        Task UpdateProgressAsync(UserProgressModel userProgress);
    }
}
