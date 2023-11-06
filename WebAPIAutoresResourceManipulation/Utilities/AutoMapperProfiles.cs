using AutoMapper;

namespace WebAPIAutoresResourceManipulation;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CreateAuthorDTO, Autor>();
    }
}
