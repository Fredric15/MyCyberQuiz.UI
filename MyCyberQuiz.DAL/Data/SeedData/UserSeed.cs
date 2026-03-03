using Microsoft.AspNetCore.Identity;
using MyCyberQuiz.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.DAL.Data.SeedData
{
    public static class UserSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager)
        {
            var userName = "user";
            var email = "user@example.com";
            var password = "Password1234!";

            //Kolla om användaren redan finns i databasen
            if (await userManager.FindByEmailAsync(email) != null)
                return;

            //Skapa användarobjektet
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            //Låt UserManager hantera hashning och sparning
            var result = await userManager.CreateAsync(user, password);

        }
    }
}
