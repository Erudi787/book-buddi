using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        public IEnumerable<GenreViewModel> GetAllGenres()
        {
            var genres = _genreRepository.GetGenres().ToList();
            return _mapper.Map<IEnumerable<GenreViewModel>>(genres);
        }

        public GenreViewModel? GetGenreById(int genreId)
        {
            var genre = _genreRepository.GetGenreById(genreId);
            return genre != null ? _mapper.Map<GenreViewModel>(genre) : null;
        }

        public IEnumerable<GenreViewModel> SearchGenres(string searchTerm)
        {
            var genres = _genreRepository.SearchGenres(searchTerm);
            return _mapper.Map<IEnumerable<GenreViewModel>>(genres);
        }

        public bool GenreExists(int genreId)
        {
            return _genreRepository.GenreExists(genreId);
        }

        public bool GenreNameExists(string genreName, int? excludeGenreId = null)
        {
            return _genreRepository.GenreNameExists(genreName, excludeGenreId);
        }

        public void AddGenre(GenreViewModel model, string createdBy)
        {
            // Check if genre name already exists
            if (_genreRepository.GenreNameExists(model.GenreName))
            {
                throw new InvalidOperationException($"A genre with the name '{model.GenreName}' already exists.");
            }

            var genre = _mapper.Map<Genre>(model);
            genre.CreatedBy = createdBy;
            genre.CreatedTime = DateTime.Now;
            genre.UpdatedBy = createdBy;
            genre.UpdatedTime = DateTime.Now;

            _genreRepository.AddGenre(genre);
        }

        public void UpdateGenre(GenreViewModel model, string updatedBy)
        {
            var genre = _genreRepository.GetGenreById(model.GenreId);
            if (genre == null)
                throw new InvalidOperationException("Genre not found");

            // Check if new name conflicts with existing genre
            if (_genreRepository.GenreNameExists(model.GenreName, model.GenreId))
            {
                throw new InvalidOperationException($"A genre with the name '{model.GenreName}' already exists.");
            }

            _mapper.Map(model, genre);
            genre.UpdatedBy = updatedBy;
            genre.UpdatedTime = DateTime.Now;

            _genreRepository.UpdateGenre(genre);
        }

        public void DeleteGenre(int genreId)
        {
            var genre = _genreRepository.GetGenreById(genreId);
            if (genre == null)
                throw new InvalidOperationException("Genre not found");

            _genreRepository.DeleteGenre(genre);
        }
    }
}
