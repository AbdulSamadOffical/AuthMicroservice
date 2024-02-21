using AuthMicroservice.Dtos;

namespace AuthMicroservice.Repositories.Interfaces
{
    public interface IUserRespository
    {
        public Task<User> GetUserById(string id);
        public Task<List<User>> GetAllUsers(PaginationParameters paginationParams);
        public Task PutUser(User user, string id);
        public Task DeleteUser(string id);
    }
}
