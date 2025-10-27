using BookBuddi.Models;

namespace BookBuddi.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(int authorId);
        Task<IEnumerable<Author>> SearchAuthorsAsync(string searchTerm);
        Task<IEnumerable<Author>> GetAuthorsByBookIdAsync(int bookId);

        Task<Author> AddAuthorAsync(Author author);
        Task UpdateAuthorAsync(Author author);
        Task DeleteAuthorAsync(int authorId);

        Task<bool> AuthorExistsAsync(int authorId);
    }
}