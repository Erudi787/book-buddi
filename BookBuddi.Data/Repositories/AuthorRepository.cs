using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class AuthorRepository : BaseRepository, IAuthorRepository
    {
        public AuthorRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Author> GetAuthors()
        {
            return this.GetDbSet<Author>().OrderBy(a => a.LastName).ThenBy(a => a.FirstName);
        }

        public Author? GetAuthorById(int authorId)
        {
            return this.GetDbSet<Author>().FirstOrDefault(a => a.AuthorId == authorId);
        }

        public IEnumerable<Author> SearchAuthors(string searchTerm)
        {
            return this.GetDbSet<Author>()
                .Where(a => a.FirstName.Contains(searchTerm) || a.LastName.Contains(searchTerm))
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public IEnumerable<Author> GetAuthorsByBookId(int bookId)
        {
            return this.GetDbSet<BookAuthor>()
                .Where(ba => ba.BookId == bookId)
                .Join(this.GetDbSet<Author>(),
                    ba => ba.AuthorId,
                    a => a.AuthorId,
                    (ba, a) => a)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public bool AuthorExists(int authorId)
        {
            return this.GetDbSet<Author>().Any(a => a.AuthorId == authorId);
        }

        public void AddAuthor(Author author)
        {
            this.GetDbSet<Author>().Add(author);
            UnitOfWork.SaveChanges();
        }

        public void UpdateAuthor(Author author)
        {
            SetEntityState(author, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteAuthor(Author author)
        {
            this.GetDbSet<Author>().Remove(author);
            UnitOfWork.SaveChanges();
        }
    }
}
