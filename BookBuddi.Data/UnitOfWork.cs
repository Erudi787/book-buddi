using BookBuddi.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContext Database { get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            Database = dbContext;
        }

        public void SaveChanges()
        {
            Database.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await Database.SaveChangesAsync();
        }

        public void Dispose()
        {
            Database?.Dispose();
        }
    }
}
