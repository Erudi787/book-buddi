using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Manager;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;
        private readonly PasswordManager _passwordManager;

        public MemberService(IMemberRepository memberRepository, IMapper mapper, PasswordManager passwordManager)
        {
            _memberRepository = memberRepository;
            _mapper = mapper;
            _passwordManager = passwordManager;
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var members = _memberRepository.GetMembers().ToList();
            return _mapper.Map<IEnumerable<MemberViewModel>>(members);
        }

        public MemberViewModel? GetMemberById(int memberId)
        {
            var member = _memberRepository.GetMemberById(memberId);
            return member != null ? _mapper.Map<MemberViewModel>(member) : null;
        }

        public MemberViewModel? GetMemberByEmail(string email)
        {
            var member = _memberRepository.GetMemberByEmail(email);
            return member != null ? _mapper.Map<MemberViewModel>(member) : null;
        }

        public IEnumerable<MemberViewModel> SearchMembers(string searchTerm)
        {
            var members = _memberRepository.SearchMembers(searchTerm);
            return _mapper.Map<IEnumerable<MemberViewModel>>(members);
        }

        public IEnumerable<MemberViewModel> GetMembersByStatus(MemberStatus status)
        {
            var members = _memberRepository.GetMembersByStatus(status);
            return _mapper.Map<IEnumerable<MemberViewModel>>(members);
        }

        public bool MemberExists(int memberId)
        {
            return _memberRepository.MemberExists(memberId);
        }

        public bool EmailExists(string email, int? excludeMemberId = null)
        {
            var existingMember = _memberRepository.GetMemberByEmail(email);
            if (existingMember == null) return false;
            if (excludeMemberId.HasValue && existingMember.MemberId == excludeMemberId.Value) return false;
            return true;
        }

        public void AddMember(MemberViewModel model, string password, string createdBy)
        {
            // Validation
            if (EmailExists(model.Email))
                throw new InvalidOperationException("A member with this email already exists");

            var validation = _passwordManager.ValidatePassword(password);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.ErrorMessage);

            var member = _mapper.Map<Member>(model);
            member.PasswordHash = _passwordManager.HashPassword(password);
            member.MembershipDate = DateTime.Now;
            member.MembershipExpiryDate = DateTime.Now.AddYears(1);
            member.Status = MemberStatus.Active;
            member.CreatedBy = createdBy;
            member.CreatedTime = DateTime.Now;
            member.UpdatedBy = createdBy;
            member.UpdatedTime = DateTime.Now;

            _memberRepository.AddMember(member);
        }

        public void UpdateMember(MemberViewModel model, string updatedBy)
        {
            if (EmailExists(model.Email, model.MemberId))
                throw new InvalidOperationException("A member with this email already exists");

            var member = _memberRepository.GetMemberById(model.MemberId);
            if (member == null)
                throw new InvalidOperationException("Member not found");

            _mapper.Map(model, member);
            member.UpdatedBy = updatedBy;
            member.UpdatedTime = DateTime.Now;

            _memberRepository.UpdateMember(member);
        }

        public void DeleteMember(int memberId)
        {
            var member = _memberRepository.GetMemberById(memberId);
            if (member == null)
                throw new InvalidOperationException("Member not found");

            _memberRepository.DeleteMember(member);
        }

        public bool ValidateCredentials(string email, string password)
        {
            var member = _memberRepository.GetMemberByEmail(email);
            if (member == null) return false;

            return _passwordManager.VerifyPassword(password, member.PasswordHash);
        }

        public void ChangePassword(int memberId, string newPassword, string updatedBy)
        {
            var validation = _passwordManager.ValidatePassword(newPassword);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.ErrorMessage);

            var member = _memberRepository.GetMemberById(memberId);
            if (member == null)
                throw new InvalidOperationException("Member not found");

            member.PasswordHash = _passwordManager.HashPassword(newPassword);
            member.UpdatedBy = updatedBy;
            member.UpdatedTime = DateTime.Now;

            _memberRepository.UpdateMember(member);
        }

        public void UpdateBorrowCount(int memberId, int change)
        {
            _memberRepository.UpdateBorrowCount(memberId, change);
        }
    }
}
