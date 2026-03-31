using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;

namespace LearnConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SubjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetSubjects()
    {
        var subjects = await _context.Subjects
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Category,
                s.Description
            })
            .ToListAsync();

        return Ok(subjects);
    }
}
