using MyCyberQuiz.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<AuthResponseDto> ChangeEmailAsync(string userId, ChangeEmailDto dto);
    }
}
