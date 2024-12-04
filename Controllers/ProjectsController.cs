using Microsoft.AspNetCore.Mvc;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TimeManagmentAPI.Controllers
{
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
        public async Task<IActionResult> GetProjects([FromQuery] string role)
        {
            if (role != "Admin")
            {
                return Unauthorized("Only admin can access this endpoint.");
            }

            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }

        // Создание проекта (только для админа)
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromQuery] string role, [FromBody] Project project)
        {
            if (role != "Admin")
            {
                return Unauthorized("Only admin can create a project.");
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return Ok(project);
        }

        // Обновление проекта (только для админа)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project updatedProject)
        {
            if (!IsAuthorized("Admin"))
            {
                return Unauthorized("Access denied");
            }

            if (id != updatedProject.Id)
            {
                return BadRequest("Project ID mismatch");
            }

            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return NotFound("Project not found");
            }

            existingProject.Name = updatedProject.Name;
            existingProject.Description = updatedProject.Description;

            _context.Entry(existingProject).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingProject);
        }

        // Удаление проекта (только для админа)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (!IsAuthorized("Admin"))
            {
                return Unauthorized("Access denied");
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Метод для проверки авторизации
        private bool IsAuthorized(string requiredRole)
        {
            var sessionId = HttpContext.Request.Headers["SessionId"].ToString();
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            HttpContext.Session.LoadAsync().Wait();

            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == requiredRole;
        }
    }
}
