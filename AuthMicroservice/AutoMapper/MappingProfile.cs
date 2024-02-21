using AuthMicroservice.Authentication;
using AuthMicroservice.Dtos;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, User>();
        
    }
}