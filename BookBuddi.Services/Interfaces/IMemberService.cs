using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.Interfaces
{
    public interface IMemberService
    {
        IEnumerable<MemberViewModel> GetAllMembers();
        MemberViewModel? GetMemberById(int memberId);
        MemberViewModel? GetMemberByEmail(string email);
        IEnumerable<MemberViewModel> SearchMembers(string searchTerm);
        IEnumerable<MemberViewModel> GetMembersByStatus(MemberStatus status);
        bool MemberExists(int memberId);
        bool EmailExists(string email, int? excludeMemberId = null);
        void AddMember(MemberViewModel model, string password, string createdBy);
        void UpdateMember(MemberViewModel model, string updatedBy);
        void DeleteMember(int memberId);
        bool ValidateCredentials(string email, string password);
        void ChangePassword(int memberId, string newPassword, string updatedBy);
        void UpdateBorrowCount(int memberId, int change);
    }
}
