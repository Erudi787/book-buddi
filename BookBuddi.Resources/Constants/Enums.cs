namespace BookBuddi.Resources.Constants
{
    /// <summary>
    /// Represents the availability of a book in the catalogue
    /// </summary>
    public enum BookStatus
    {
        Available = 1,
        Unavailable = 2,
        Archived = 3
    }

    /// <summary>
    /// Represents the status of a library member account
    /// </summary>
    public enum MemberStatus
    {
        Active = 1,
        Suspended = 2,
        Expired = 3,
        Inactive = 4
    }

    /// <summary>
    /// Represents the status of a borrow transaction
    /// </summary>
    public enum TransactionStatus
    {
        Active = 1,
        Returned = 2,
        Overdue = 3,
        Lost = 4
    }

    /// <summary>
    /// Represents the status of a fine
    /// </summary>
    public enum FineStatus
    {
        Unpaid = 1,
        Paid = 2,
        Waived = 3,
        Disputed = 4
    }

    /// <summary>
    /// Represents the status of a book request
    /// </summary>
    public enum RequestStatus
    {
        Pending = 1,
        Approved = 2,
        Fulfilled = 3,
        Rejected = 4,
        Cancelled = 5
    }

    /// <summary>
    /// Represents the type of notification
    /// </summary>
    public enum NotificationType
    {
        DueReminder = 1,
        OverdueAlert = 2,
        RequestUpdate = 3,
        FineIssued = 4,
        General = 5
    }

    /// <summary>
    /// Represents the reason for a fine
    /// </summary>
    public enum FineReason
    {
        Overdue = 1,
        LostBook = 2,
        Damaged = 3
    }
}
