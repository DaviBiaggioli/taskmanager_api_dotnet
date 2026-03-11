using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanager_api_dotnet.Data;
using taskmanager_api_dotnet.Models;
using taskmanager_api_dotnet.DTOs;

namespace taskmanager_api_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context) => _context = context;

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = userDto.Password //por enquanto implementado sem criptografia, será corrigido quando for implementado o JWT
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserResponseDto { Id = user.Id, Name = user.Name, Email = user.Email });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserRegisterDto loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null || user.PasswordHash != loginRequest.Password)
            {
                return Unauthorized("Invalido");
            }

            return Ok(new { message = $"Bem-vindo, {user.Name}!" });
        }
    }
}