using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public IEnumerable<AuthorViewModel> GetAllAuthors()
        {
            var authors = _authorRepository.GetAuthors().ToList();
            return _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
        }

        public AuthorViewModel? GetAuthorById(int authorId)
        {
            var author = _authorRepository.GetAuthorById(authorId);
            return author != null ? _mapper.Map<AuthorViewModel>(author) : null;
        }

        public IEnumerable<AuthorViewModel> SearchAuthors(string searchTerm)
        {
            var authors = _authorRepository.SearchAuthors(searchTerm);
            return _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
        }

        public IEnumerable<AuthorViewModel> GetAuthorsByBookId(int bookId)
        {
            var authors = _authorRepository.GetAuthorsByBookId(bookId);
            return _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
        }

        public bool AuthorExists(int authorId)
        {
            return _authorRepository.AuthorExists(authorId);
        }

        public void AddAuthor(AuthorViewModel model, string createdBy)
        {
            var author = _mapper.Map<Author>(model);
            author.CreatedBy = createdBy;
            author.CreatedTime = DateTime.Now;
            author.UpdatedBy = createdBy;
            author.UpdatedTime = DateTime.Now;

            _authorRepository.AddAuthor(author);
        }

        public void UpdateAuthor(AuthorViewModel model, string updatedBy)
        {
            var author = _authorRepository.GetAuthorById(model.AuthorId);
            if (author == null)
                throw new InvalidOperationException("Author not found");

            _mapper.Map(model, author);
            author.UpdatedBy = updatedBy;
            author.UpdatedTime = DateTime.Now;

            _authorRepository.UpdateAuthor(author);
        }

        public void DeleteAuthor(int authorId)
        {
            var author = _authorRepository.GetAuthorById(authorId);
            if (author == null)
                throw new InvalidOperationException("Author not found");

            _authorRepository.DeleteAuthor(author);
        }
    }
}
