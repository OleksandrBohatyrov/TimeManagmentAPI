using Microsoft.AspNetCore.Mvc;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Использование LINQ
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks; // Явное использование System.Threading.Tasks

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

        // Получение всех задач (для администратора или общего доступа)
        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = _context.Tasks.ToList();
            return Ok(tasks);
        }

        // Добавление задачи (доступно только для Админа)
        [HttpPost("addTask")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTask([FromBody] ManagedTask task) // Изменено на ManagedTask
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok("Task added successfully");
        }

        // Получение задач конкретного пользователя (требуется авторизация)
        [HttpGet("userTasks")]
        [Authorize]
        public IActionResult GetUserTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userTasks = _context.Tasks.Where(t => t.UserId == userId).ToList();
            return Ok(userTasks);
        }

        // Создание задачи (может использоваться обычным пользователем)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] ManagedTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        // Обновление задачи по ID
        [HttpPut("{id}")]
        [Authorize]
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

            // Обновляем поля задачи
            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.ProjectId = updatedTask.ProjectId;
            existingTask.UserId = updatedTask.UserId;
            existingTask.StartTime = updatedTask.StartTime;
            existingTask.EndTime = updatedTask.EndTime;
            existingTask.IsCompleted = updatedTask.IsCompleted;
            existingTask.IsConfirmed = updatedTask.IsConfirmed;

            _context.Entry(existingTask).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingTask);
        }

        // Удаление задачи по ID (доступно только для Админа)
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
