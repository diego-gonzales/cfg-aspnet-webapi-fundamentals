using AutoMapper;

namespace WebAPIAutoresResourceManipulation;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CreateAuthorDTO, Author>();
        CreateMap<Author, AuthorDTO>();

        CreateMap<CreateBookDTO, Book>();
        CreateMap<Book, BookDTO>();

        CreateMap<CreateCommentDTO, Comment>();
        CreateMap<Comment, CommentDTO>();
        // El siguiente código es solo en caso tengan nombres distintos en las propiedades
        // CreateMap<Autor, CreateAuthorDTO>()
        //     .ForMember(dto => dto.Poliza, ent => ent.MapFrom(prop => prop.NRO_POLIZA));
    }
}
