using AutoMapper;

namespace WebAPIAutoresResourceManipulation;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CreateAuthorDTO, Autor>();
        CreateMap<Autor, AuthorDTO>();
        CreateMap<CreateBookDTO, Libro>();
        CreateMap<Libro, BookDTO>();

        // El siguiente código es solo en caso tengan nombres distintos en las propiedades
        // CreateMap<Autor, CreateAuthorDTO>()
        //     .ForMember(dto => dto.Poliza, ent => ent.MapFrom(prop => prop.NRO_POLIZA));
    }
}
