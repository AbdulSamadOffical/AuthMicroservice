using AuthMicroservice.Dtos;
using AuthMicroservice.Services;
using AuthMicroservice.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;


namespace AuthMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {

        private readonly IJsonWebTokenService<JwtSecurityToken, IdentityResult> _authService;
        private readonly UserService _userservice;
        public AuthenticateController(IJsonWebTokenService<JwtSecurityToken, IdentityResult> authService, UserService userservice)
        {
            _authService = authService;
            _userservice = userservice;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {

                var token = await _authService.GenerateToken(model);
                return Ok(new Response<string>()
                {
                    Data = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Token Created Sucessfully"
                });
        }
            
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
         
                await _authService.Register(model);
                return Ok(new Response<string>()
                {
                    Data = null,
                    Message = "User Registered Sucessfully."
                });
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userservice.GetUserById(id);
            return Ok(new Response<User>()
            {
                Data = user,
                Message = ""
            });

        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParameters paginationParams)
        {
            var user = await _userservice.GetAllUsers(paginationParams); 
            return Ok(new ResponseList<User>()
            {
                Data = user,
                Message = ""
            });

        }
        [HttpPut("user/{id}")]
        public async Task<IActionResult> PutUser(User user, string id)
        {
           await _userservice.PutUser(user, id);
            return Ok(new ResponseList<User>()
            {
                Data = null,
                Message = "User updated sucessfully"
            });

        }
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser (string id)
        {
            await _userservice.DeleteUser(id);
            return Ok(new ResponseList<User>()
            {
                Data = null,
                Message = "User deleted sucessfully"
            });

        }

    }
}
