using MyCyberQuiz.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}
