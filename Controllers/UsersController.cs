using Microsoft.AspNetCore.Mvc;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace TimeManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TimeManagementContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(TimeManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Username == registerUserDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
                Role = registerUserDto.Role
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, registerUserDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Registered Successfully");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == loginUserDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid Username or Password");
            }

            var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
            if (passwordVerification != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid Username or Password");
            }

            // Генерация JWT-токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Добавление токена в куки
            HttpContext.Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { Token = tokenString });
        }


    }

    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LoginUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
