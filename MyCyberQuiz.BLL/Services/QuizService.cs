using MyCyberQuiz.DAL.Repositories.Interfaces;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using MyCyberQuiz.DAL.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MyCyberQuiz.BLL.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUserProgressRepository _userProgressRepository; // För att kunna spara användarprogress i framtiden
        private readonly IUserScoreRepository _userScoreRepository; // För att kunna spara användarpoäng i framtiden
        private readonly UserManager<ApplicationUser> _userManager;
        

        public QuizService(IQuizRepository quizRepository, UserManager<ApplicationUser> userManager, IUserProgressRepository userProgressRepository, IUserScoreRepository userScoreRepository)
        {
            _quizRepository = quizRepository;
            _userManager = userManager;
            _userProgressRepository = userProgressRepository;
            _userScoreRepository = userScoreRepository;
        }
        public async Task<IEnumerable<CategoryDto>> GetMenuCategoriesAsync(string userId)
        {
            var categoryDtos = new List<CategoryDto>();
            
            // 1. Hämta användarens alla framsteg - om inga framsteg, skapa ny UserProgressModel

            var userProgress = string.IsNullOrEmpty(userId)
                ? new List<UserProgressModel>()
                : await _userProgressRepository.GetUserProgressByUserIdAsync(userId);

            // 2. Hämta alla kategorier och underkategorier
            var categories = await _quizRepository.GetAllCategoriesWithSubCAsync();

            foreach (var category in categories)
            {
                var subCategoryDtos = new List<SubCategoryDto>();

                // Sortera underkategorierna på Order så vi vet vilken som är första nivån
                var sortedSubCategories = category.SubCategories.OrderBy(sc => sc.Order).ToList();

                for (int i = 0; i < sortedSubCategories.Count; i++)
                {
                    var sc = sortedSubCategories[i];
                    bool isLocked = true; // Som standard låser vi nivån

                    // Den allra första underkategorin (i varje huvudkategori) ska alltid vara öppen från start
                    if (i == 0)
                    {
                        isLocked = false;
                    }
                    // För övriga nivåer: Kolla om din Submit-metod tidigare har skapat en rad för denna nivå
                    else if (userProgress.Any(p => p.SubCategoryModelId == sc.Id))
                    {
                        isLocked = false;
                    }

                    subCategoryDtos.Add(new SubCategoryDto(
                        sc.Id,
                        sc.Name,
                        sc.Description,
                        isLocked,
                        sc.Order
                    ));
                }

                categoryDtos.Add(new CategoryDto(
                    category.Id,
                    category.Name,
                    category.Description,
                    subCategoryDtos
                ));
            }

            return categoryDtos;
        }
        public async Task<QuizDetailsDto> GetQuizByIdAsync(int subCategoryId)
        {
            // Hämtar subkategorin och inkluderar Quizzes -> Questions -> Options
            var subCategory = await _quizRepository.GetCompleteQuizByIdAsync(subCategoryId);

            // Vi antar här att varje subkategori har ett huvudquiz (första i listan)
            var quiz = subCategory?.Quizzes.FirstOrDefault();

            if (quiz == null) return null;

            // Mappa till DTO
            return new QuizDetailsDto(
                quiz.Id,
                quiz.Text,
                quiz.Description,
                quiz.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = q.Options.Select(o => new OptionDto(o.Id, o.Text)).ToList()
                }).ToList()
            );
        }

        public async Task<QuizResultDto> SubmitQuizAsync(SubmitQuizDto submission, string userId)
        {

            // 1. Hämta hela quizet (inklusive facit) från databasen
            var subCategory = await _quizRepository.GetCompleteQuizByIdAsync(submission.SubCategoryId);

            // Hitta rätt quiz (vi antar att det är det första för subkategorin)
            var quiz = subCategory?.Quizzes.FirstOrDefault(q => q.Id == submission.QuizId);

            if (quiz == null)
            {
                throw new Exception("Kunde inte hitta quizet för rättning.");
            }

            int correctAnswers = 0;
            int totalQuestions = quiz.Questions.Count;

            // 2. Rätta svaren!
            foreach (var question in quiz.Questions)
            {
                // Hitta det korrekta alternativet i databasen för just denna fråga
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

                // Hitta det svar användaren skickade in för samma fråga
                var userAnswer = submission.Answers.FirstOrDefault(a => a.QuestionId == question.Id);

                // Om användaren har svarat, och svaret matchar facit -> Ge 1 poäng!
                if (correctOption != null && userAnswer != null)
                {
                    if (correctOption.Id == userAnswer.SelectedOptionId)
                    {
                        correctAnswers++;
                    }
                }
            }

            // 3. Räkna ut om användaren blev godkänd (t.ex. minst 80% rätt)
            double scorePercentage = (double)correctAnswers / totalQuestions;
            bool passed = scorePercentage >= 0.8;

            // Spara poängen i databasen
            UserScoreModel scoreModel = new UserScoreModel
            {
                ApplicationUserId = userId,
                ApplicationUser = await _userManager.FindByIdAsync(userId),
                QuizModelId = quiz.Id,
                Score = correctAnswers,
                TotalQuestions = totalQuestions,
                CompletedAt = DateTime.UtcNow
            };
            await _userScoreRepository.AddScoreAsync(scoreModel);

            // SPARA PROGRESS & LÅS UPP NÄSTA NIVÅ 
            if (passed && userId != null)
            {
                // 1. Markera den nuvarande underkategorin som avklarad
                var currentProgress = await _userProgressRepository.GetProgressAsync(userId, submission.SubCategoryId);

                if (currentProgress == null)
                {
                    // Skapa ny rad om den inte fanns
                    await _userProgressRepository.AddProgressAsync(new UserProgressModel
                    {
                        ApplicationUserId = userId,
                        SubCategoryModelId = submission.SubCategoryId,
                        IsCompleted = true
                    });
                }
                else if (!currentProgress.IsCompleted)
                {
                    // Uppdatera befintlig rad
                    currentProgress.IsCompleted = true;
                    await _userProgressRepository.UpdateProgressAsync(currentProgress);
                }

                // 2. Lås upp NÄSTA underkategori!
                // Vi frågar QuizRepository vilket ID som kommer härnäst i samma huvudkategori
                var nextSubCategoryId = await _quizRepository.GetNextSubCategoryIdAsync(submission.SubCategoryId);

                if (nextSubCategoryId != null)
                {
                    // Kolla om användaren redan har låst upp den
                    var nextProgress = await _userProgressRepository.GetProgressAsync(userId, nextSubCategoryId.Value);

                    if (nextProgress == null)
                    {
                        // Lås upp genom att skapa en rad där IsCompleted = false
                        await _userProgressRepository.AddProgressAsync(new UserProgressModel
                        {
                            ApplicationUserId = userId,
                            SubCategoryModelId = nextSubCategoryId.Value,
                            IsCompleted = false, // Den är upplåst, men ej klarad än!
                            UnlockedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            // 4. Skicka tillbaka resultatet
            return new QuizResultDto(
                submission.QuizId,
                totalQuestions,
                correctAnswers,
                passed,
                passed ? "Snyggt jobbat! Du blev godkänd!" : "Tyvärr, du nådde inte gränsen för godkänt. Försök igen!"
            );
        }

        public async Task<AnswerFeedbackDto> CheckAnswerAsync(int questionId, int selectedOptionId)
        {
            // 1. Hämta frågan med dess svarsalternativ från databasen
            // (Byt ut detta mot hur din repository hämtar en fråga, t.ex. GetQuestionByIdAsync)
            var question = await _quizRepository.GetQuestionByIdAsync(questionId);

            if (question == null)
                throw new Exception("Frågan hittades inte.");

            // 2. Leta fram det rätta alternativet i databasen (där IsCorrect == true)
            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

            if (correctOption == null)
                throw new Exception("Frågan saknar ett rätt svar i databasen.");

            // 3. Kolla om användarens valda ID matchar det rätta ID:t
            bool isCorrect = (selectedOptionId == correctOption.Id);

            // 4. Skicka tillbaka domen!
            return new AnswerFeedbackDto(isCorrect, correctOption.Id, correctOption.Text);
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            // 1. Hämta användaren för att kunna visa upp e-post
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            { return null; }

            // 2. Hämta all progress för denna användare
            var userProgress = await _userProgressRepository.GetUserProgressByUserIdAsync(userId);

            // Räkna hur många de faktiskt har klarat (IsCompleted == true)
            int completedCount = userProgress.Count(p => p.IsCompleted);

            // Hämta totala antalet subkategorier som finns i hela spelet
            var allCategories = await _quizRepository.GetAllCategoriesWithSubCAsync();
            int totalSubCategoriesCount = allCategories.SelectMany(c => c.SubCategories).Count();

            // 3. Hämta användarens historik/poäng)
            var userScores = await _userScoreRepository.GetUserScoreByUserIdAsync(userId);

            // 4. Bygg ihop hela paketet och skicka till UI!
            var profile = new UserProfileDto
            {
                Email = user.Email,
                CompletedSubCategories = completedCount,
                TotalSubCategories = totalSubCategoriesCount,
                TotalQuizzesPlayed = userScores.Count(),
                TotalCorrectAnswers = userScores.Sum(s => s.Score),

                // Mappa poängen till historik-DTO:n (sortera så nyaste ligger först)
                RecentHistory = userScores.OrderByDescending(s => s.CompletedAt).Select(s => new QuizHistoryDto
                {
                    QuizName = "Quiz ID: " + s.Quiz.Text,
                    Score = s.Score,
                    TotalQuestions = s.TotalQuestions,
                    CompletedAt = s.CompletedAt,
                    Passed = ((double)s.Score / s.TotalQuestions) >= 0.8
                }).ToList()
            };

            return profile;
        }
    }
}
