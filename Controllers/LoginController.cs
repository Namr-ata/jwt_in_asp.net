using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public IActionResult Login([FromBody] User user)
        {
            if(user.Username=="admin" && user.Password=="password") {
                var token = GenerateJwtToken(user);
                return Ok(token);
            }
            return BadRequest("invalid user");
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = Encoding.UTF8.GetBytes(_configuration["Jwt:secret"]);

            var claims = new Claim[] {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Email,user.Username)
                };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(7),
              signingCredentials: credentials);


            

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
