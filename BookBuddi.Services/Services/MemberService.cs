using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Manager;
using BookBuddi.Services.ServiceModels;
using System.Security.Cryptography;

namespace BookBuddi.Services.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;
        private readonly PasswordManager _passwordManager;
        private readonly Data.ApplicationDbContext _context;

        public MemberService(IMemberRepository memberRepository, IMapper mapper, PasswordManager passwordManager, Data.ApplicationDbContext context)
        {
            _memberRepository = memberRepository;
            _mapper = mapper;
            _passwordManager = passwordManager;
            _context = context;
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var members = _memberRepository.GetMembers().ToList();
            return _mapper.Map<IEnumerable<MemberViewModel>>(members);
        }

        public PagedResult<MemberViewModel> GetMembersPaged(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _memberRepository.GetMembers();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m =>
                    m.FirstName.Contains(searchTerm) ||
                    m.LastName.Contains(searchTerm) ||
                    m.Email.Contains(searchTerm));
            }

            var totalCount = query.Count();
            var members = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var memberViewModels = _mapper.Map<List<MemberViewModel>>(members);
            return new PagedResult<MemberViewModel>(memberViewModels, totalCount, pageNumber, pageSize);
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

        public string GeneratePasswordResetToken(string email)
        {
            var member = _memberRepository.GetMemberByEmail(email);
            if (member == null)
                throw new InvalidOperationException("No account found with this email address.");

            // Generate a secure random token
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

            // Save token to database
            var resetToken = new PasswordResetToken
            {
                MemberId = member.MemberId,
                Token = token,
                CreatedTime = DateTime.Now,
                ExpiryTime = DateTime.Now.AddHours(1), // Token valid for 1 hour
                IsUsed = false
            };

            _context.PasswordResetTokens.Add(resetToken);
            _context.SaveChanges();

            return token;
        }

        public bool ValidatePasswordResetToken(string token)
        {
            var resetToken = _context.PasswordResetTokens
                .FirstOrDefault(t => t.Token == token && !t.IsUsed && t.ExpiryTime > DateTime.Now);

            return resetToken != null;
        }

        public void ResetPassword(string token, string newPassword)
        {
            var resetToken = _context.PasswordResetTokens
                .FirstOrDefault(t => t.Token == token && !t.IsUsed && t.ExpiryTime > DateTime.Now);

            if (resetToken == null)
                throw new InvalidOperationException("Invalid or expired reset token.");

            // Validate password strength
            var validation = _passwordManager.ValidatePassword(newPassword);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.ErrorMessage);

            // Get member and update password
            var member = _memberRepository.GetMemberById(resetToken.MemberId);
            if (member == null)
                throw new InvalidOperationException("Member not found.");

            member.PasswordHash = _passwordManager.HashPassword(newPassword);
            member.UpdatedBy = "System";
            member.UpdatedTime = DateTime.Now;

            // Mark token as used
            resetToken.IsUsed = true;

            _memberRepository.UpdateMember(member);
            _context.SaveChanges();
        }

        public MemberViewModel? GetMemberByVerificationToken(string token)
        {
            Console.WriteLine($"[MemberService] Searching for member with token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");

            var member = _context.Members
                .FirstOrDefault(m => m.EmailVerificationToken == token);

            if (member != null)
            {
                Console.WriteLine($"[MemberService] Found member: {member.Email}, Token in DB: {member.EmailVerificationToken?.Substring(0, Math.Min(20, member.EmailVerificationToken?.Length ?? 0))}...");
            }
            else
            {
                Console.WriteLine($"[MemberService] No member found with this token");
                // Check if any members exist with verification tokens
                var anyWithToken = _context.Members.Any(m => m.EmailVerificationToken != null);
                Console.WriteLine($"[MemberService] Members with verification tokens in DB: {anyWithToken}");
            }

            return member != null ? _mapper.Map<MemberViewModel>(member) : null;
        }

        public MemberViewModel? GetMemberByVerificationCode(string code)
        {
            var member = _context.Members
                .FirstOrDefault(m => m.EmailVerificationCode == code);

            return member != null ? _mapper.Map<MemberViewModel>(member) : null;
        }
    }
}
