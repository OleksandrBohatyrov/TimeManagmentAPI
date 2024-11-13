using Microsoft.AspNetCore.Mvc;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace TimeManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

          
            if (User.Identity.Name != task.UserId.ToString() && User.FindFirst("Role")?.Value != "Admin")
            {
                return Forbid(); 
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (task.IsConfirmed && User.FindFirst("Role")?.Value == "User")
            {
                return BadRequest("Cannot delete confirmed task");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
