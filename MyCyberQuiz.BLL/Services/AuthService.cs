using MyCyberQuiz.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.BLL.Settings;
using MyCyberQuiz.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyCyberQuiz.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                //Skapa innehållet i JWT-token
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                //Hämta nyckeln och skapa token
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                //Skapa token
                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                //Returnera token som en sträng
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return new AuthResponseDto(true, null, tokenString);
            }
            return new AuthResponseDto(false, "Ogiltig e-post eller lösenord", null);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto(false, "En användare med denna e-postadress finns redan.", null);
            }

            var NewUser = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };
            
            var result = await _userManager.CreateAsync(NewUser, registerDto.Password);

            if (!result.Succeeded)
            {
                // Om det misslyckas (t.ex. för svagt lösenord), plocka ut felmeddelandena
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto(false, errors, null);
            }

            if (result.Succeeded)
            {
                return new AuthResponseDto(true, null, null);
            }
            return new AuthResponseDto(false, "Registrering misslyckades", null);
        }
    }
}
