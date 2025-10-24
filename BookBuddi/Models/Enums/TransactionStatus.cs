namespace BookBuddi.Models.Enums
{
    public enum TransactionStatus
    {
        Active = 1, // Book is currently borrowed and not yet returned
        Returned = 2, // Book has been returned
        Overdue = 3, // Book is overdue (past due date for return)
        Lost = 4 // Book has been reported lost
    }
}