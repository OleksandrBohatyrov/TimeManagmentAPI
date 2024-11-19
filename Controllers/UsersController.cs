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
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Username == registerUserRequest.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = registerUserRequest.Username,
                Email = registerUserRequest.Email,
                Role = registerUserRequest.Role
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, registerUserRequest.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Registered Successfully");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginRequest userLogin)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == userLogin.Username);
            if (user == null)
            {
                return Unauthorized("Invalid Username or Password");
            }

            var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userLogin.Password);
            if (passwordVerification != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid Username or Password");
            }

            return Ok("Login successful");
        }
    }



}
