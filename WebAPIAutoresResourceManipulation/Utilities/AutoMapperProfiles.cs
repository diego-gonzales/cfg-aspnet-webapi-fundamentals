using AutoMapper;

namespace WebAPIAutoresResourceManipulation;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CreateAuthorDTO, Author>();
        CreateMap<Author, AuthorDTO>();
        CreateMap<Author, AuthorWithBooksDTO>()
            .ForMember(
                authorWithBooksDto => authorWithBooksDto.Books,
                options => options.MapFrom(MapToBookDtos)
            );

        CreateMap<CreateBookDTO, Book>()
            .ForMember(book => book.AuthorsBooks, options => options.MapFrom(MapToAuthorsBooks)); // (🚩🚩🌍🌍)
        CreateMap<Book, BookDTO>();
        CreateMap<Book, BookWithCommentsAndAuthorsDTO>()
            .ForMember(
                bookWithCommentsAndAuthorsDTO => bookWithCommentsAndAuthorsDTO.Autores,
                options => options.MapFrom(MapToAuthorDtos)
            );

        CreateMap<CreateCommentDTO, Comment>();
        CreateMap<Comment, CommentDTO>();
        CreateMap<Comment, CommentWithBookDTO>();

        // El siguiente código es solo en caso tengan nombres distintos en las propiedades, por ejemplo:
        // CreateMap<Autor, CreateAuthorDTO>()
        //     .ForMember(dto => dto.Poliza, ent => ent.MapFrom(prop => prop.NRO_POLIZA));
    }

    private List<AuthorBook> MapToAuthorsBooks(CreateBookDTO createBookDTO, Book book) // (🚩🚩🌍🌍)
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

    private List<AuthorDTO> MapToAuthorDtos(
        Book book,
        BookWithCommentsAndAuthorsDTO bookWithCommentsAndAuthorsDTO
    )
    {
        var result = new List<AuthorDTO>();

        if (book.AuthorsBooks == null)
        {
            return result;
        }

        foreach (var authorBook in book.AuthorsBooks)
        {
            result.Add(new AuthorDTO() { Id = authorBook.AuthorId, Name = authorBook.Author.Name });
        }

        return result;
    }

    private List<BookDTO> MapToBookDtos(Author author, AuthorWithBooksDTO authorWithBooksDTO)
    {
        var result = new List<BookDTO>();

        if (author.AuthorsBooks == null)
        {
            return null;
        }

        foreach (var authorBook in author.AuthorsBooks)
        {
            result.Add(new BookDTO() { Id = authorBook.BookId, Name = authorBook.Book.Name });
        }

        return result;
    }
}

/*
    CreateMap<CreateBookDTO, Book>()
        .ForMember(book => book.AuthorsBooks, options => options.MapFrom(MapAuthorsBooks));

    // aquí hacemos esto debido a que la entidad 'Book' tenemos entre sus propiedades: 'public List<AuthorBook> AuthorsBooks { get; set; }'; y como la entidad 'CreateBookDto' trae un arreglo de ids de autores: 'public List<int> AuthorIds { get; set; }'; entonces tenemos que hacer esa tranformación y lo hacemo dentro del método 'MapToAuthorsBooks()' el cual va a devolver ese 'List<AuthorBook>'
*/

/*
    CreateMap<Book, BookWithCommentsAndAuthorsDTO>()
            .ForMember(
                bookWithCommentsAndAuthorsDTO => bookWithCommentsAndAuthorsDTO.Autores,
                options => options.MapFrom(MapToAuthorDtos)
            );

    // aquí hacemos esto debido a que la entidad 'BookWithCommentsAndAuthorsDTO' tenemos entre sus propiedades: 'public List<AuthorDTO> Autores { get; set; }'; y como la entidad 'Book' tiene una propiedad 'public List<AuthorBook> AuthorsBooks { get; set; }'; entonces tenemos que hacer esa tranformación y lo hacemo dentro del método 'MapToAuthorDtos()' el cual va a devolver ese 'List<AuthorDTO>'
*/
