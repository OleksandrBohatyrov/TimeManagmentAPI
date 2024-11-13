using Microsoft.AspNetCore.Mvc;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly TimeManagementContext _context;

    public ProjectsController(TimeManagementContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetProjects()
    {
        return Ok(_context.Projects.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return Ok(project);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
