using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{
    // Record nyare variant av en klass, som är mer optimerad för immutability och enklare syntax
    public record CategoryDto(int Id, string Name, string Description, List<SubCategoryDto> SubCategories);

    public record SubCategoryDto(int Id, string Name, string Description, bool IsLocked, int Order);
}
