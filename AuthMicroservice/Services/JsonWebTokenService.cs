using AuthMicroservice.Authentication;
using AuthMicroservice.Dtos;
using AuthMicroservice.Exceptions;
using AuthMicroservice.MessageBroker.RabbitMq.Interfaces;
using AuthMicroservice.Repositories.Interfaces;
using AuthMicroservice.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthMicroservice.Services
{
    public class JsonWebTokenService: IJsonWebTokenService<JwtSecurityToken, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;
        private readonly ILogger<JsonWebTokenService> _logger;
        private readonly IUserRespository _userRespository;
        public JsonWebTokenService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IBus bus, ILogger<JsonWebTokenService> logger, IUserRespository userRespository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _bus = bus;
            _logger = logger;
            _userRespository = userRespository;
        }

        public async Task<JwtSecurityToken> GenerateToken(LoginModel model)
        {
           
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user == null)
                {
                    throw new BadRequestException("user doesn't exist's.");
                }

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    throw new BadRequestException("Incorrect password.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim("user_id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );
                return token;
            }
          
           
            
        

        public async Task<IdentityResult> Register(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                throw new NotFoundException("user doesn't exist's.");

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            var message = new User() { UserName = model.Username , Email = model.Email};
            await _bus.SendAsync<User>("user", message);
            _logger.LogInformation($"Message produced sucessfully Username: {user.UserName}, Stock Symbol: {user.Email}");
            return result;
        }
       
    }

    
}
