using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
            var admin = new User
            {
                Id = model.Id,
                UserName = model.UserName,
                Email = model.Email,
                Role = "Admin"
            };

            var passwordHasher = new PasswordHasher<User>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, model.Password);

            var result = await _userManager.CreateAsync(admin);
            if (result.Succeeded)
            {
                return Ok("Admin user created successfully.");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Role = model.Role 
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("User registered successfully");
            }

            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null)
            {
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

                if (passwordValid)
                {
                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    return Ok(new { userId = user.Id, role = user.Role });
                }
            }

            return Unauthorized("Invalid login attempt");
        }


        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        public class AdminRequest
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
