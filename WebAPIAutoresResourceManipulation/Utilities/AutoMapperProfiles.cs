using AutoMapper;

namespace WebAPIAutoresResourceManipulation;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CreateAuthorDTO, Author>();
        CreateMap<Author, AuthorDTO>();

        CreateMap<CreateBookDTO, Book>()
            .ForMember(book => book.AuthorsBooks, options => options.MapFrom(MapAuthorsBooks)); // (🚩🚩🌍🌍)
        CreateMap<Book, BookDTO>();

        CreateMap<CreateCommentDTO, Comment>();
        CreateMap<Comment, CommentDTO>();

        // El siguiente código es solo en caso tengan nombres distintos en las propiedades, por ejemplo:
        // CreateMap<Autor, CreateAuthorDTO>()
        //     .ForMember(dto => dto.Poliza, ent => ent.MapFrom(prop => prop.NRO_POLIZA));
    }

    private List<AuthorBook> MapAuthorsBooks(CreateBookDTO createBookDTO, Book book) // (🚩🚩🌍🌍)
    {
        var result = new List<AuthorBook>();

        if (createBookDTO.AuthorIds == null)
        {
            return result;
        }

        foreach (var authorId in createBookDTO.AuthorIds)
        {
            result.Add(new AuthorBook() { AuthorId = authorId }); // no le agregamos el BookId ya que de eso se va a encargar EFCore cuando se cree el libro, ya que es obvio que antes de crear el libro no tenemos el id de dicho libro.
            // Con respecto a la pregunta de los comentarios del curso: en que momento se insertan los registros AuthorBook en la base de datos? ya que solo se hace el dbContext.add(book)? ver respuesta de Felipe Gavilán: (https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/26946920#questions/19704328)
        }

        return result;
    }
}
