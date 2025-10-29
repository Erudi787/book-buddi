using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Database { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
