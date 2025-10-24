namespace BookBuddi.Models.Enums
{
    public enum RequestStatus
    {
        Pending = 1,
        Approved = 2, // approved by admin
        Fulfilled = 3, // book has been acquired and available
        Rejected = 4,
        Cancelled = 5,

    }
}