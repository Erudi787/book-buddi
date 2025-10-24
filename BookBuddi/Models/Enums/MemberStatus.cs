namespace BookBuddi.Models.Enums
{
    public enum MemberStatus
    {
        Active = 1, // Member account is active and in good standing
        Suspented = 2, // Member account suspended (e.g., overdue books, unpaid fines)
        Expired = 3, // Memebership expired
        Inactive = 4 // Member account has been deactivated
    }
}