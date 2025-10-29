using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            return await _context.Members
                .OrderBy(m => m.LastName)
                .ThenBy(m => m.FirstName)
                .ToListAsync();
        }

        public async Task<Member?> GetMemberByIdAsync(int memberId)
        {
            return await _context.Members
                .Include(m => m.BorrowTransactions)
                .Include(m => m.Fines)
                .Include(m => m.BookRequests)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);
        }

        public async Task<Member?> GetMemberByEmailAsync(string email)
        {
            return await _context.Members
                .FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm)
        {
            return await _context.Members
                .Where(m => m.FirstName.Contains(searchTerm) ||
                           m.LastName.Contains(searchTerm) ||
                           m.Email.Contains(searchTerm))
                .OrderBy(m => m.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Member>> GetMembersByStatusAsync(MemberStatus status)
        {
            return await _context.Members
                .Where(m => m.Status == status)
                .OrderBy(m => m.LastName)
                .ToListAsync();
        }

        public async Task<Member> AddMemberAsync(Member member)
        {
            member.MembershipDate = DateTime.Now;
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task UpdateMemberAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMemberAsync(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> MemberExistsAsync(int memberId)
        {
            return await _context.Members.AnyAsync(m => m.MemberId == memberId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Members.AnyAsync(m => m.Email == email);
        }

        public async Task UpdateBorrowedCountAsync(int memberId, int change)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                member.CurrentBorrowedCount += change;
                await _context.SaveChangesAsync();
            }
        }

        public Task<Member?> GetMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }
    }
}