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
    public class UsersController : ControllerBase
    {
        private readonly TimeManagementContext _context;

        public UsersController(TimeManagementContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, user.Password);
            user.Password = null;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Registered Successfully");
        }

        [HttpPost("login")]
        public IActionResult Login(User userInput)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == userInput.Username);
            if (user == null)
            {
                return Unauthorized("Invalid Username or Password");
            }

            var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userInput.Password);
            if (passwordVerification != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid Username or Password");
            }

            return Ok("Login successful");
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Username = updatedUser.Username;
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                existingUser.PasswordHash = new PasswordHasher<User>().HashPassword(existingUser, updatedUser.Password);
            }

            existingUser.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return Ok(existingUser);
        }
    }
}
