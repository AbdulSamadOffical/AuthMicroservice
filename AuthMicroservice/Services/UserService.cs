using AuthMicroservice.Dtos;
using AuthMicroservice.Repositories;
using AuthMicroservice.Repositories.Interfaces;
using AuthMicroservice.Services.IServices;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
namespace AuthMicroservice.Services
{
    public class UserService
    {

        
        private readonly IUserRespository _userRespository;
        public UserService( IUserRespository userRespository) 
        {
            _userRespository = userRespository;
          
        }
        public async Task<User> GetUserById(string id)
        {
            var user = await _userRespository.GetUserById(id);
            return user;
        }

        public async Task<List<User>> GetAllUsers(PaginationParameters paginationParams)
        {
            return await _userRespository.GetAllUsers(paginationParams);
        }
        public async Task PutUser(User user, string id)
        {
             await _userRespository.PutUser(user, id );
        }
        public async Task DeleteUser(string id)
        {
           await _userRespository.DeleteUser(id);
        }
    }
}
