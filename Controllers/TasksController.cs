using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace TimeManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TimeManagementContext _context;

        public TasksController(TimeManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            return Ok(_context.Tasks.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TimeTask task)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }

            if (User.FindFirst(ClaimTypes.Name)?.Value != task.UserId.ToString() && User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
            {
                return Forbid();
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
