using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using TimeManagmentAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace TimeManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }



        [HttpPost("addAdmin")]
        public async Task<IActionResult> AddAdmin([FromBody] AdminRequest model)
        {
            // Создаем объект пользователя
            var admin = new User
            {
                Id = model.Id,
                UserName = model.UserName,
                Email = model.Email,
                Role = model.Role
            };

            // Хешируем пароль
            var passwordHasher = new PasswordHasher<User>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, model.Password);

            // Сохраняем пользователя в базе данных через UserManager
            var result = await _userManager.CreateAsync(admin);
            if (result.Succeeded)
            {
                return Ok("Admin user created successfully.");
            }

            return BadRequest(result.Errors);
        }


        // Регистрация пользователя
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Role = model.Role // если у вас есть роль
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("User registered successfully");
            }

            return BadRequest(result.Errors);
        }

        // Вход пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null)
            {
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

                if (passwordValid)
                {
                    // Создаем новую сессию и сохраняем данные пользователя
                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Возвращаем идентификатор сессии клиенту
                    var sessionId = HttpContext.Session.Id;

                    return Ok(new { sessionId, username = user.UserName, role = user.Role });
                }
            }

            return Unauthorized("Invalid login attempt");
        }

        // Получение всех пользователей (только для админа)
        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!IsAuthorized("Admin"))
            {
                return Unauthorized("Access denied");
            }

            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
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



        public class AdminRequest
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
        }

    }
}
