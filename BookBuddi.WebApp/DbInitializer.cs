using Microsoft.AspNetCore.Identity;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Manager;

namespace BookBuddi.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<Admin> userManager,
            RoleManager<IdentityRole> roleManager,
            PasswordManager passwordManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Books.Any())
            {
                return; // Database has been seeded
            }

            // Create Admin Role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create Admin User
            var admin = new Admin
            {
                UserName = "admin@bookbuddi.com",
                Email = "admin@bookbuddi.com",
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true,
                DateCreated = DateTime.Now,
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now
            };

            var adminPassword = "Admin@123";
            var adminUser = await userManager.FindByEmailAsync(admin.Email);
            if (adminUser == null)
            {
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed Categories
            var categories = new Category[]
            {
                new Category { CategoryName = "Fiction", Description = "Literary works of imagination", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Category { CategoryName = "Non-Fiction", Description = "Factual and informative works", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Category { CategoryName = "Reference", Description = "Reference materials and encyclopedias", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Category { CategoryName = "Children", Description = "Books for children and young readers", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Category { CategoryName = "Academic", Description = "Scholarly and educational texts", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Seed Genres
            var genres = new Genre[]
            {
                new Genre { GenreName = "Biography", GenreDescription = "Life stories of real people", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Mystery", GenreDescription = "Detective and crime stories", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Romance", GenreDescription = "Love stories and relationships", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Science Fiction", GenreDescription = "Futuristic and speculative fiction", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Fantasy", GenreDescription = "Magical and imaginary worlds", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "History", GenreDescription = "Historical accounts and analysis", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Science", GenreDescription = "Scientific knowledge and discoveries", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Technology", GenreDescription = "Technology and computing", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Self-Help", GenreDescription = "Personal development and improvement", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Genre { GenreName = "Business", GenreDescription = "Business and economics", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.Genres.AddRange(genres);
            await context.SaveChangesAsync();

            // Seed Authors
            var authors = new Author[]
            {
                new Author { FirstName = "Jane", LastName = "Austen", DateOfBirth = new DateTime(1775, 12, 16), Nationality = "British", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "George", LastName = "Orwell", DateOfBirth = new DateTime(1903, 6, 25), Nationality = "British", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "J.K.", LastName = "Rowling", DateOfBirth = new DateTime(1965, 7, 31), Nationality = "British", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "Isaac", LastName = "Asimov", DateOfBirth = new DateTime(1920, 1, 2), Nationality = "American", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "Agatha", LastName = "Christie", DateOfBirth = new DateTime(1890, 9, 15), Nationality = "British", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "Stephen", LastName = "King", DateOfBirth = new DateTime(1947, 9, 21), Nationality = "American", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "Malcolm", LastName = "Gladwell", DateOfBirth = new DateTime(1963, 9, 3), Nationality = "Canadian", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "Yuval Noah", LastName = "Harari", DateOfBirth = new DateTime(1976, 2, 24), Nationality = "Israeli", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "Walter", LastName = "Isaacson", DateOfBirth = new DateTime(1952, 5, 20), Nationality = "American", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Author { FirstName = "Michelle", LastName = "Obama", DateOfBirth = new DateTime(1964, 1, 17), Nationality = "American", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.Authors.AddRange(authors);
            await context.SaveChangesAsync();

            // Seed Books
            var books = new Book[]
            {
                new Book { ISBN = "9780141439518", BookTitle = "Pride and Prejudice", Publisher = "Penguin Classics", PublicationDate = new DateTime(1813, 1, 28), Description = "A romantic novel of manners", PageCount = 432, AvailableCopies = 5, CategoryId = 1, GenreId = 3, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9780451524935", BookTitle = "1984", Publisher = "Signet Classic", PublicationDate = new DateTime(1949, 6, 8), Description = "A dystopian social science fiction novel", PageCount = 328, AvailableCopies = 7, CategoryId = 1, GenreId = 4, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9780439708180", BookTitle = "Harry Potter and the Sorcerer's Stone", Publisher = "Scholastic", PublicationDate = new DateTime(1998, 9, 1), Description = "The first novel in the Harry Potter series", PageCount = 309, AvailableCopies = 10, CategoryId = 4, GenreId = 5, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9780553293357", BookTitle = "Foundation", Publisher = "Bantam Spectra", PublicationDate = new DateTime(1951, 6, 1), Description = "First book in the Foundation series", PageCount = 255, AvailableCopies = 4, CategoryId = 1, GenreId = 4, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9780062073488", BookTitle = "Murder on the Orient Express", Publisher = "William Morrow", PublicationDate = new DateTime(1934, 1, 1), Description = "A detective novel featuring Hercule Poirot", PageCount = 256, AvailableCopies = 6, CategoryId = 1, GenreId = 2, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9780316769174", BookTitle = "The Catcher in the Rye", Publisher = "Little, Brown and Company", PublicationDate = new DateTime(1951, 7, 16), Description = "A story about teenage rebellion", PageCount = 277, AvailableCopies = 5, CategoryId = 1, GenreId = 3, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9780316346627", BookTitle = "Outliers", Publisher = "Little, Brown and Company", PublicationDate = new DateTime(2008, 11, 18), Description = "The Story of Success", PageCount = 309, AvailableCopies = 5, CategoryId = 2, GenreId = 9, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9780062316097", BookTitle = "Sapiens", Publisher = "Harper", PublicationDate = new DateTime(2015, 2, 10), Description = "A Brief History of Humankind", PageCount = 443, AvailableCopies = 8, CategoryId = 2, GenreId = 6, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9781501127625", BookTitle = "Steve Jobs", Publisher = "Simon & Schuster", PublicationDate = new DateTime(2011, 10, 24), Description = "The biography of Steve Jobs", PageCount = 656, AvailableCopies = 4, CategoryId = 2, GenreId = 1, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Book { ISBN = "9781524763138", BookTitle = "Becoming", Publisher = "Crown", PublicationDate = new DateTime(2018, 11, 13), Description = "Memoir by Michelle Obama", PageCount = 448, AvailableCopies = 6, CategoryId = 2, GenreId = 1, Status = BookStatus.Available, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            // Seed BookAuthors
            var bookAuthors = new BookAuthor[]
            {
                new BookAuthor { BookId = 1, AuthorId = 1, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 2, AuthorId = 2, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 3, AuthorId = 3, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 4, AuthorId = 4, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 5, AuthorId = 5, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 7, AuthorId = 7, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 8, AuthorId = 8, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 9, AuthorId = 9, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookAuthor { BookId = 10, AuthorId = 10, AuthorOrder = 1, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.BookAuthors.AddRange(bookAuthors);
            await context.SaveChangesAsync();

            // Seed Members
            string defaultPassword = "Password123!";
            string hashedPassword = passwordManager.HashPassword(defaultPassword);

            var members = new Member[]
            {
                new Member { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PasswordHash = hashedPassword, Phone = "555-1234", Address = "123 Main St, City", DateOfBirth = new DateTime(1990, 5, 15), Status = MemberStatus.Active, BorrowingLimit = 5, MembershipDate = DateTime.Now, MembershipExpiryDate = DateTime.Now.AddYears(1), CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Member { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", PasswordHash = hashedPassword, Phone = "555-5678", Address = "456 Oak Ave, Town", DateOfBirth = new DateTime(1985, 8, 22), Status = MemberStatus.Active, BorrowingLimit = 5, MembershipDate = DateTime.Now, MembershipExpiryDate = DateTime.Now.AddYears(1), CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Member { FirstName = "Robert", LastName = "Johnson", Email = "robert.j@example.com", PasswordHash = hashedPassword, Phone = "555-9012", Address = "789 Pine Rd, Village", DateOfBirth = new DateTime(1995, 3, 10), Status = MemberStatus.Active, BorrowingLimit = 3, MembershipDate = DateTime.Now, MembershipExpiryDate = DateTime.Now.AddYears(1), CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Member { FirstName = "Emily", LastName = "Williams", Email = "emily.w@example.com", PasswordHash = hashedPassword, Phone = "555-3456", Address = "321 Elm St, City", DateOfBirth = new DateTime(1992, 11, 5), Status = MemberStatus.Suspended, BorrowingLimit = 5, MembershipDate = DateTime.Now, MembershipExpiryDate = DateTime.Now.AddYears(1), CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Member { FirstName = "Michael", LastName = "Brown", Email = "michael.b@example.com", PasswordHash = hashedPassword, Phone = "555-7890", Address = "654 Maple Dr, Town", DateOfBirth = new DateTime(1988, 7, 18), Status = MemberStatus.Active, BorrowingLimit = 5, MembershipDate = DateTime.Now, MembershipExpiryDate = DateTime.Now.AddYears(1), CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.Members.AddRange(members);
            await context.SaveChangesAsync();

            // Seed BorrowTransactions
            var transactions = new BorrowTransaction[]
            {
                new BorrowTransaction { BookId = 1, MemberId = 1, BorrowDate = DateTime.Now.AddDays(-10), DueDate = DateTime.Now.AddDays(4), Status = TransactionStatus.Active, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BorrowTransaction { BookId = 2, MemberId = 2, BorrowDate = DateTime.Now.AddDays(-20), DueDate = DateTime.Now.AddDays(-6), Status = TransactionStatus.Overdue, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BorrowTransaction { BookId = 3, MemberId = 1, BorrowDate = DateTime.Now.AddDays(-30), DueDate = DateTime.Now.AddDays(-16), ReturnDate = DateTime.Now.AddDays(-15), Status = TransactionStatus.Returned, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.BorrowTransactions.AddRange(transactions);
            await context.SaveChangesAsync();

            // Update book availability
            var book1 = context.Books.Find(1);
            if (book1 != null) book1.AvailableCopies = 4;
            var book2 = context.Books.Find(2);
            if (book2 != null) book2.AvailableCopies = 6;

            // Update member borrowed counts
            var member1 = context.Members.Find(1);
            if (member1 != null) member1.CurrentBorrowedCount = 1;
            var member2 = context.Members.Find(2);
            if (member2 != null) member2.CurrentBorrowedCount = 1;

            await context.SaveChangesAsync();

            // Seed BookRequests
            var requests = new BookRequest[]
            {
                new BookRequest { MemberId = 3, BookTitle = "The Hobbit", AuthorName = "J.R.R. Tolkien", RequestDate = DateTime.Now, Status = RequestStatus.Pending, MemberNotes = "Would love to read this classic!", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new BookRequest { MemberId = 1, BookTitle = "Dune", AuthorName = "Frank Herbert", ISBN = "9780441172719", RequestDate = DateTime.Now, Status = RequestStatus.Approved, AdminNotes = "Will order next week", CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.BookRequests.AddRange(requests);
            await context.SaveChangesAsync();

            // Seed Notifications
            var notifications = new Notification[]
            {
                new Notification { MemberId = 1, Type = NotificationType.DueReminder, Title = "Book Due Soon", Message = "Your borrowed book 'Pride and Prejudice' is due in 4 days.", DateCreated = DateTime.Now, IsRead = false, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Notification { MemberId = 2, Type = NotificationType.OverdueAlert, Title = "Book Overdue", Message = "Your borrowed book '1984' is overdue. Please return it as soon as possible.", DateCreated = DateTime.Now, IsRead = false, CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now },
                new Notification { MemberId = 1, Type = NotificationType.RequestUpdate, Title = "Book Request Approved", Message = "Your request for 'Dune' has been approved!", DateCreated = DateTime.Now, IsRead = true, DateRead = DateTime.Now.AddDays(-2), CreatedBy = "System", CreatedTime = DateTime.Now, UpdatedBy = "System", UpdatedTime = DateTime.Now }
            };
            context.Notifications.AddRange(notifications);
            await context.SaveChangesAsync();

            // Seed Fine
            var fine = new Fine
            {
                TransactionId = 2,
                MemberId = 2,
                Amount = 6.00m,
                Reason = FineReason.Overdue,
                Status = FineStatus.Unpaid,
                IssueDate = DateTime.Now,
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now
            };
            context.Fines.Add(fine);
            await context.SaveChangesAsync();
        }
    }
}
