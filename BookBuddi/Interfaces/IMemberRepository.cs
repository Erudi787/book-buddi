using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllMembersAsync();
        Task<Member?> GetMemberIdAsync(int memberId);
        Task<Member?> GetMemberByEmailAsync(string email);
        Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm);
        Task<IEnumerable<Member>> GetMembersByStatusAsync(MemberStatus status);

        Task<Member> AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
        Task DeleteMemberAsync(int memberId);

        Task<bool> MemberExistsAsync(int memberId);
        Task<bool> EmailExistsAsync(string email);
        Task UpdateBorrowedCountAsync(int memberId, int change);
    }
}