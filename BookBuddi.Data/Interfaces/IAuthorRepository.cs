using BookBuddi.Data.Models;

namespace BookBuddi.Data.Interfaces
{
    public interface IAuthorRepository
    {
        IQueryable<Author> GetAuthors();
        Author? GetAuthorById(int authorId);
        IEnumerable<Author> SearchAuthors(string searchTerm);
        IEnumerable<Author> GetAuthorsByBookId(int bookId);
        bool AuthorExists(int authorId);
        void AddAuthor(Author author);
        void UpdateAuthor(Author author);
        void DeleteAuthor(Author author);
    }
}
