using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class MemberRepository : BaseRepository, IMemberRepository
    {
        public MemberRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Member> GetMembers()
        {
            return this.GetDbSet<Member>().OrderBy(m => m.LastName).ThenBy(m => m.FirstName);
        }

        public Member? GetMemberById(int memberId)
        {
            return this.GetDbSet<Member>().FirstOrDefault(m => m.MemberId == memberId);
        }

        public Member? GetMemberByEmail(string email)
        {
            return this.GetDbSet<Member>().FirstOrDefault(m => m.Email == email);
        }

        public IEnumerable<Member> SearchMembers(string searchTerm)
        {
            return this.GetDbSet<Member>()
                .Where(m => m.FirstName.Contains(searchTerm) ||
                           m.LastName.Contains(searchTerm) ||
                           m.Email.Contains(searchTerm))
                .OrderBy(m => m.LastName)
                .ToList();
        }

        public IEnumerable<Member> GetMembersByStatus(MemberStatus status)
        {
            return this.GetDbSet<Member>()
                .Where(m => m.Status == status)
                .OrderBy(m => m.LastName)
                .ToList();
        }

        public bool MemberExists(int memberId)
        {
            return this.GetDbSet<Member>().Any(m => m.MemberId == memberId);
        }

        public bool EmailExists(string email)
        {
            return this.GetDbSet<Member>().Any(m => m.Email == email);
        }

        public void AddMember(Member member)
        {
            this.GetDbSet<Member>().Add(member);
            UnitOfWork.SaveChanges();
        }

        public void UpdateMember(Member member)
        {
            SetEntityState(member, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteMember(Member member)
        {
            this.GetDbSet<Member>().Remove(member);
            UnitOfWork.SaveChanges();
        }

        public void UpdateBorrowCount(int memberId, int change)
        {
            var member = GetMemberById(memberId);
            if (member != null)
            {
                member.CurrentBorrowedCount += change;
                UpdateMember(member);
            }
        }
    }
}
