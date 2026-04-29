using Microsoft.AspNetCore.Mvc;

namespace SkillBridge.Api.Controllers;

[ApiController]
[Route("[controller]")]
public IEnumerable<JobDto> GetJoblist()
    {
        return new List<JobDto>
        {
           new JobDto (Id = 1, Name = "Software Engineer",
Description = "Develop and maintain software applications"},
new JobDto {Id = 2, Name = "product Manager",
Description = "Manage product development and strategy" }
        };
    }
}