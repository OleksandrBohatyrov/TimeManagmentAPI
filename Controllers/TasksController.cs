using Microsoft.AspNetCore.Mvc;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            var tasks = _context.Tasks.ToList();
            return Ok(tasks);
        }

        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask([FromBody] ManagedTask task)
        {
            if (task == null)
            {
                return BadRequest("Task object is null");
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok("Task added successfully");
        }

        [HttpGet("userTasks")]
        public IActionResult GetUserTasks([FromQuery] string userId)
        {
            var userTasks = _context.Tasks.Where(t => t.UserId == userId).ToList();
            return Ok(userTasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] ManagedTask updatedTask)
        {
            if (id != updatedTask.Id)
            {
                return BadRequest("Task ID mismatch");
            }

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound("Task not found");
            }

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.ProjectId = updatedTask.ProjectId;
            existingTask.StartTime = updatedTask.StartTime;
            existingTask.EndTime = updatedTask.EndTime;
            existingTask.IsCompleted = updatedTask.IsCompleted;
            existingTask.IsConfirmed = updatedTask.IsConfirmed;

            _context.Entry(existingTask).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingTask);
        }

        [HttpDelete("{id}")]
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
