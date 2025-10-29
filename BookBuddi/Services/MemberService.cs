using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Services
{
    public class MemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IFineRepository _fineRepository;

        public MemberService(IMemberRepository memberRepository, IFineRepository fineRepository)
        {
            _memberRepository = memberRepository;
            _fineRepository = fineRepository;
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            return await _memberRepository.GetAllMembersAsync();
        }

        public async Task<Member?> GetMemberByIdAsync(int memberId)
        {
            return await _memberRepository.GetMemberIdAsync(memberId);
        }

        public async Task<Member?> GetMemberByEmailAsync(string email)
        {
            return await _memberRepository.GetMemberByEmailAsync(email);
        }

        public async Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _memberRepository.GetAllMembersAsync();
            }

            return await _memberRepository.SearchMembersAsync(searchTerm);
        }

        public async Task<IEnumerable<Member>> GetActiveMembersAsync()
        {
            return await _memberRepository.GetMembersByStatusAsync(MemberStatus.Active);
        }

        public async Task<Member> RegisterMemberAsync(Member member)
        {
            // Validation
            if (await _memberRepository.EmailExistsAsync(member.Email))
            {
                throw new InvalidOperationException($"A member with email {member.Email} already exists.");
            }

            // Set initial values
            member.MembershipDate = DateTime.Now;
            member.Status = MemberStatus.Active;
            member.BorrowingLimit = 5; // Default limit
            member.CurrentBorrowedCount = 0;

            return await _memberRepository.AddMemberAsync(member);
        }

        public async Task UpdateMemberAsync(Member member)
        {
            var existingMember = await _memberRepository.GetMemberIdAsync(member.MemberId);
            if (existingMember == null)
            {
                throw new InvalidOperationException("Member not found.");
            }

            await _memberRepository.UpdateMemberAsync(member);
        }

        public async Task SuspendMemberAsync(int memberId, string reason)
        {
            var member = await _memberRepository.GetMemberIdAsync(memberId);
            if (member == null)
            {
                throw new InvalidOperationException("Member not found.");
            }

            member.Status = MemberStatus.Suspented;
            await _memberRepository.UpdateMemberAsync(member);
        }

        public async Task ReactivateMemberAsync(int memberId)
        {
            var member = await _memberRepository.GetMemberIdAsync(memberId);
            if (member == null)
            {
                throw new InvalidOperationException("Member not found.");
            }

            // Check if member has unpaid fines
            var unpaidFines = await _fineRepository.GetUnpaidFinesByMemberIdAsync(memberId);
            if (unpaidFines.Any())
            {
                throw new InvalidOperationException("Cannot reactivate member with unpaid fines.");
            }

            member.Status = MemberStatus.Active;
            await _memberRepository.UpdateMemberAsync(member);
        }

        public async Task<bool> CanBorrowBooksAsync(int memberId)
        {
            var member = await _memberRepository.GetMemberIdAsync(memberId);
            if (member == null)
                return false;

            if (member.Status != MemberStatus.Active)
                return false;

            if (member.CurrentBorrowedCount >= member.BorrowingLimit)
                return false;

            var unpaidFines = await _fineRepository.GetUnpaidFinesByMemberIdAsync(memberId);
            if (unpaidFines.Any())
                return false;

            return true;
        }
    }
}
