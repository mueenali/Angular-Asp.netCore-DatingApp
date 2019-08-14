using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            this._authRepo = authRepo;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            //validate request 
            userRegisterDto.Username = userRegisterDto.Username.ToLower();
            if (await _authRepo.UserFound(userRegisterDto.Username))
            {
                return BadRequest("User already exists");
            }
            var user = await _authRepo.Register(new User { Username = userRegisterDto.Username }, userRegisterDto.Password);
            return StatusCode(201);
        }
    }
}