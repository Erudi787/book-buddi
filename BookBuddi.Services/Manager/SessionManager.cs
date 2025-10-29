using Microsoft.AspNetCore.Http;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.Manager
{
    public class SessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        // User Role
        public string? UserRole
        {
            get => Session?.GetString(Const.SessionKeyUserRole);
            set
            {
                if (Session != null && value != null)
                    Session.SetString(Const.SessionKeyUserRole, value);
            }
        }

        // Member ID
        public int? MemberId
        {
            get => Session?.GetInt32(Const.SessionKeyMemberId);
            set
            {
                if (Session != null && value.HasValue)
                    Session.SetInt32(Const.SessionKeyMemberId, value.Value);
            }
        }

        // Member Name
        public string? MemberName
        {
            get => Session?.GetString(Const.SessionKeyMemberName);
            set
            {
                if (Session != null && value != null)
                    Session.SetString(Const.SessionKeyMemberName, value);
            }
        }

        // Admin Name
        public string? AdminName
        {
            get => Session?.GetString(Const.SessionKeyAdminName);
            set
            {
                if (Session != null && value != null)
                    Session.SetString(Const.SessionKeyAdminName, value);
            }
        }

        // Admin Email
        public string? AdminEmail
        {
            get => Session?.GetString(Const.SessionKeyAdminEmail);
            set
            {
                if (Session != null && value != null)
                    Session.SetString(Const.SessionKeyAdminEmail, value);
            }
        }

        // Helper methods
        public bool IsAdmin => UserRole == Const.AdminRole;

        public bool IsMember => UserRole == Const.MemberRole;

        public bool IsAuthenticated => !string.IsNullOrEmpty(UserRole);

        public void Clear()
        {
            Session?.Clear();
        }

        public void SetMemberSession(int memberId, string memberName)
        {
            UserRole = Const.MemberRole;
            MemberId = memberId;
            MemberName = memberName;
        }

        public void SetAdminSession(string adminName, string adminEmail)
        {
            UserRole = Const.AdminRole;
            AdminName = adminName;
            AdminEmail = adminEmail;
        }
    }
}
