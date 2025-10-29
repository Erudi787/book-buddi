namespace BookBuddi.Resources.Constants
{
    public static class Const
    {
        // Authentication
        public const string AuthenticationScheme = "BookBuddiAuth";
        public const string AdminRole = "Admin";
        public const string MemberRole = "Member";

        // Session Keys
        public const string SessionKeyUserRole = "UserRole";
        public const string SessionKeyMemberId = "MemberId";
        public const string SessionKeyMemberName = "MemberName";
        public const string SessionKeyAdminName = "AdminName";
        public const string SessionKeyAdminEmail = "AdminEmail";

        // Default Values
        public const int DefaultBorrowingLimit = 5;
        public const int DefaultBorrowPeriodDays = 14;
        public const decimal DefaultOverdueFinePerDay = 1.00m;

        // Validation
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 100;
        public const int MaxTitleLength = 300;
        public const int MaxNameLength = 100;
        public const int MaxEmailLength = 200;
        public const int MaxIsbnLength = 13;

        // Messages
        public const string SuccessMessage = "Operation completed successfully.";
        public const string ErrorMessage = "An error occurred. Please try again.";
        public const string ValidationErrorMessage = "Please correct the errors and try again.";
        public const string UnauthorizedMessage = "You are not authorized to perform this action.";
        public const string NotFoundMessage = "The requested resource was not found.";
    }
}
