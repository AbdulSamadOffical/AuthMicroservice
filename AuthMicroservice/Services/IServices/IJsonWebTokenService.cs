using AuthMicroservice.Dtos;


namespace AuthMicroservice.Services.IServices
{
    public interface IJsonWebTokenService<T,K>
    {

        Task<T> GenerateToken(LoginModel model);
        Task<K> Register(RegisterModel model);
        

    }
}
