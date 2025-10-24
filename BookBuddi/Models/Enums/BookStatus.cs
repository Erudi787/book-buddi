namespace BookBuddi.Models.Enums
{
    /// <summary>
    /// Represents the availability of a book in the catalogue
    /// </summary>
    public enum BookStatus
    {
        Available = 1, // Book is available for borrowing
        Unavailable = 2, // Book is currently unavailable (all copies borrowed or no stock)
        Archived = 3 // Book is archived and is no longer in active circulation
    }
}