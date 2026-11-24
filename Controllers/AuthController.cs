
using Microsoft.AspNetCore.Mvc;

namespace CampusChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly List<User> Users = new()
        {
            new User { Id = 101, Name = "Ahmed", Username = "ahmed", Password = "1234" },
            new User { Id = 202, Name = "Omar", Username = "omar", Password = "1234" },
            new User { Id = 303, Name = "Sara", Username = "sara", Password = "1234" }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = Users.FirstOrDefault(u =>
                u.Username == request.Username &&
                u.Password == request.Password);

            if (user == null)
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });

            HttpContext.Session.SetInt32("UserId", user.Id);

            return Ok(new
            {
                userId = user.Id,
                name = user.Name,
                message = "Login successful"
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                message = "Logout successful"
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
