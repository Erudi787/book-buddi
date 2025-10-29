using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Interfaces
{
    public interface IMemberRepository
    {
        IQueryable<Member> GetMembers();
        Member? GetMemberById(int memberId);
        Member? GetMemberByEmail(string email);
        IEnumerable<Member> SearchMembers(string searchTerm);
        IEnumerable<Member> GetMembersByStatus(MemberStatus status);
        bool MemberExists(int memberId);
        bool EmailExists(string email);
        void AddMember(Member member);
        void UpdateMember(Member member);
        void DeleteMember(Member member);
        void UpdateBorrowCount(int memberId, int change);
    }
}
