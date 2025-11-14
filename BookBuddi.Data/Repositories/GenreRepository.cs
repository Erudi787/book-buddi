using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class GenreRepository : BaseRepository, IGenreRepository
    {
        public GenreRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Genre> GetGenres()
        {
            return this.GetDbSet<Genre>().OrderBy(g => g.GenreName);
        }

        public Genre? GetGenreById(int genreId)
        {
            return this.GetDbSet<Genre>().FirstOrDefault(g => g.GenreId == genreId);
        }

        public IEnumerable<Genre> SearchGenres(string searchTerm)
        {
            return this.GetDbSet<Genre>()
                .Where(g => g.GenreName.Contains(searchTerm) ||
                           (g.GenreDescription != null && g.GenreDescription.Contains(searchTerm)))
                .OrderBy(g => g.GenreName)
                .ToList();
        }

        public bool GenreExists(int genreId)
        {
            return this.GetDbSet<Genre>().Any(g => g.GenreId == genreId);
        }

        public bool GenreNameExists(string genreName, int? excludeGenreId = null)
        {
            var query = this.GetDbSet<Genre>()
                .Where(g => g.GenreName.ToLower() == genreName.ToLower());

            if (excludeGenreId.HasValue)
            {
                query = query.Where(g => g.GenreId != excludeGenreId.Value);
            }

            return query.Any();
        }

        public void AddGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Add(genre);
            UnitOfWork.SaveChanges();
        }

        public void UpdateGenre(Genre genre)
        {
            SetEntityState(genre, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Remove(genre);
            UnitOfWork.SaveChanges();
        }
    }
}
