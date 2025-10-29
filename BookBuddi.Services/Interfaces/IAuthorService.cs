using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Interfaces
{
    public interface IAuthorService
    {
        IEnumerable<AuthorViewModel> GetAllAuthors();
        AuthorViewModel? GetAuthorById(int authorId);
        IEnumerable<AuthorViewModel> SearchAuthors(string searchTerm);
        IEnumerable<AuthorViewModel> GetAuthorsByBookId(int bookId);
        bool AuthorExists(int authorId);
        void AddAuthor(AuthorViewModel model, string createdBy);
        void UpdateAuthor(AuthorViewModel model, string updatedBy);
        void DeleteAuthor(int authorId);
    }
}
