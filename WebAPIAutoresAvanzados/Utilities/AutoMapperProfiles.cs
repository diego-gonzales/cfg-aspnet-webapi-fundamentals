using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace WebAPIAutoresAvanzados;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CreateAuthorDTO, Author>();
        CreateMap<UpdateAuthorDTO, Author>();
        CreateMap<Author, AuthorDTO>();
        CreateMap<Author, AuthorWithBooksDTO>()
            .ForMember(
                authorWithBooksDto => authorWithBooksDto.Books,
                options => options.MapFrom(MapToBookDtos)
            );

        CreateMap<CreateBookDTO, Book>()
            .ForMember(book => book.AuthorsBooks, options => options.MapFrom(MapToAuthorsBooks));
        CreateMap<Book, BookDTO>();
        CreateMap<Book, BookWithCommentsAndAuthorsDTO>()
            .ForMember(
                bookWithCommentsAndAuthorsDTO => bookWithCommentsAndAuthorsDTO.Autores,
                options => options.MapFrom(MapToAuthorDtos)
            );
        CreateMap<PatchBookDTO, Book>().ReverseMap();

        CreateMap<CreateCommentDTO, Comment>();
        CreateMap<UpdateCommentDTO, Comment>();
        CreateMap<Comment, CommentDTO>();
        CreateMap<Comment, CommentWithBookDTO>();

        CreateMap<IdentityUser, IdentityUserDTO>();
    }

    private List<AuthorBook> MapToAuthorsBooks(CreateBookDTO createBookDTO, Book book)
    {
        var result = new List<AuthorBook>();

        if (createBookDTO.AuthorIds == null)
        {
            return result;
        }

        foreach (var authorId in createBookDTO.AuthorIds)
        {
            result.Add(new AuthorBook() { AuthorId = authorId });
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
