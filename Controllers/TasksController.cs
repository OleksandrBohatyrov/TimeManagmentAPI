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

        // Получение всех задач (только для админа)
        [HttpGet]
        public IActionResult GetTasks()
        {
            if (!IsAuthorized("Admin"))
            {
                return Unauthorized("Access denied");
            }

            var tasks = _context.Tasks.ToList();
            return Ok(tasks);
        }

        // Добавление задачи (только для админа)
        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask([FromBody] ManagedTask task)
        {
            if (!IsAuthorized("Admin"))
            {
                return Unauthorized("Access denied");
            }

            if (task == null)
            {
                return BadRequest("Task object is null");
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok("Task added successfully");
        }

        // Получение задач текущего пользователя
        [HttpGet("userTasks")]
        public IActionResult GetUserTasks()
        {
            if (!IsAuthorized())
            {
                return Unauthorized("Access denied");
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User not found");
            }

            var userTasks = _context.Tasks.Where(t => t.UserId == userIdString).ToList();
            return Ok(userTasks);
        }

        // Обновление задачи (только для авторизованных пользователей)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] ManagedTask updatedTask)
        {
            if (!IsAuthorized())
            {
                return Unauthorized("Access denied");
            }

            if (id != updatedTask.Id)
            {
                return BadRequest("Task ID mismatch");
            }

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound("Task not found");
            }

            // Проверяем, что пользователь имеет доступ к этой задаче
            var userId = HttpContext.Session.GetString("UserId");
            if (existingTask.UserId != userId)
            {
                return Unauthorized("You do not have access to this task");
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

        // Удаление задачи (только для админа)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (!IsAuthorized("Admin"))
            {
                return Unauthorized("Access denied");
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Метод для проверки авторизации
        private bool IsAuthorized(string requiredRole = null)
        {
            var sessionId = HttpContext.Request.Headers["SessionId"].ToString();
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            HttpContext.Session.LoadAsync().Wait();

            if (requiredRole == null)
            {
                var userId = HttpContext.Session.GetString("UserId");
                return !string.IsNullOrEmpty(userId);
            }
            else
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                return userRole == requiredRole;
            }
        }
    }
}
