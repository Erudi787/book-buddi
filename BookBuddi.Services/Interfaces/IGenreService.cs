using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Interfaces
{
    public interface IGenreService
    {
        IEnumerable<GenreViewModel> GetAllGenres();
        GenreViewModel? GetGenreById(int genreId);
        IEnumerable<GenreViewModel> SearchGenres(string searchTerm);
        bool GenreExists(int genreId);
        bool GenreNameExists(string genreName, int? excludeGenreId = null);
        void AddGenre(GenreViewModel model, string createdBy);
        void UpdateGenre(GenreViewModel model, string updatedBy);
        void DeleteGenre(int genreId);
    }
}
