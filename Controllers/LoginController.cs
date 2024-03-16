
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TokenAuthDemo.Models;

namespace TokenAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] User user)
        {
            // Check if the ID is provided in the request body
            /*
            if (string.IsNullOrWhiteSpace(user.ID))
            {
                // If ID is not provided, generate a new unique identifier
                user.ID = Guid.NewGuid().ToString();
            }
            */
            // Check user credentials
            if (user.Username == "admin" && user.Password == "password")
            {
                var token = GenerateJwtToken(user);
                return Ok(token);
            }

            return BadRequest("Invalid username or password");
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = Encoding.UTF8.GetBytes(_configuration["Jwt:secret"]);

            var claims = new Claim[] {
               // new Claim(ClaimTypes.Name, user.ID.ToString()), // Using ID for the Name claim
                new Claim(ClaimTypes.NameIdentifier, user.Username) // Using Username for the NameIdentifier claim
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
