using AuthMicroservice.Authentication;
using AuthMicroservice.Database;
using AuthMicroservice.Dtos;
using AuthMicroservice.Exceptions;
using AuthMicroservice.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.Repositories
{
    public class UserRepository: IUserRespository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UserRepository(UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext context) 
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context; 
        }

        public async Task<User> GetUserById(string id)
        {
            var identityUser = await _userManager.FindByIdAsync(id) ?? throw new NotFoundException("user doesn't exists.");
            var user = _mapper.Map<ApplicationUser,User>(identityUser);
            return user;
        }

        public async Task<List<User>> GetAllUsers(PaginationParameters paginationParams)
        {
            var identityUsers =  await _userManager.Users.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                              .Take(paginationParams.PageSize).ToListAsync();
            var users = _mapper.Map<List<ApplicationUser>, List<User>>(identityUsers);
            return users;
        }

        public async Task PutUser(User user, string id)
        {
            var identityUser = await _userManager.FindByIdAsync(id)?? throw new NotFoundException("user doesn't exists");
            identityUser.UserName = user.UserName;
            identityUser.Email = user.Email;
            await _userManager.UpdateAsync(identityUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(string id)
        {
            var identityUser = await _userManager.FindByIdAsync(id) ?? throw new NotFoundException("user doesn't exists");
            await _userManager.DeleteAsync(identityUser);   
        }
    }
}
