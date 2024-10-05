using Business.Category;
using Business.DTO.Auth;
using Business.DTO.Create;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;

        public AuthController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO userDTO)
        {
            var result = await _userBusiness.Register(userDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // Login a user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            var result = await _userBusiness.Login(loginUserDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            return Unauthorized(result);
        }

        // Refresh the JWT token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string token)
        {
            var result = await _userBusiness.RefreshToken(token);
            if (result.Success)
            {
                return Ok(result);
            }
            return Unauthorized(result);
        }
    }
}
