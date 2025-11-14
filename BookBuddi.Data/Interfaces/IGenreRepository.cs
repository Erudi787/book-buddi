using BookBuddi.Data.Models;

namespace BookBuddi.Data.Interfaces
{
    public interface IGenreRepository
    {
        IQueryable<Genre> GetGenres();
        Genre? GetGenreById(int genreId);
        IEnumerable<Genre> SearchGenres(string searchTerm);
        bool GenreExists(int genreId);
        bool GenreNameExists(string genreName, int? excludeGenreId = null);
        void AddGenre(Genre genre);
        void UpdateGenre(Genre genre);
        void DeleteGenre(Genre genre);
    }
}
