# Book Buddi - Team Setup Instructions

> **Purpose**: Instructions for team members to set up the Book Buddi project on their local machines.

---

## Prerequisites

Before you start, make sure you have installed:

1. ✅ **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download)
2. ✅ **Visual Studio Code** - [Download here](https://code.visualstudio.com/)
3. ✅ **C# Dev Kit Extension** - Install from VS Code Extensions (Ctrl+Shift+X)
4. ✅ **SQL Server** (Express or higher) - [Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
5. ✅ **SQL Server Management Studio (SSMS)** - [Download here](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
6. ✅ **Git** - [Download here](https://git-scm.com/)

---

## Step-by-Step Setup

### Step 1: Clone the Repository

Open your terminal/command prompt and run:

```bash
git clone https://github.com/Erudi787/book-buddi.git
cd book-buddi
```

### Step 2: Identify Your SQL Server Instance

Open **SQL Server Management Studio (SSMS)** and note the server name in the connection dialog.

Common SQL Server instance names:
- `(local)`
- `localhost`
- `.\SQLEXPRESS`
- `(localdb)\MSSQLLocalDB`

**Write down your instance name - you'll need it in Step 4!**

### Step 3: Restore NuGet Packages

Restore all required packages for all projects (BookBuddi, BookBuddi.Data, BookBuddi.Services, BookBuddi.Resources):

```bash
dotnet restore
```

You should see output indicating packages are being restored for multiple projects.

### Step 4: Update Connection String

**Open the file**: `BookBuddi.WebApp/appsettings.json`

Update the `Server=` part with YOUR SQL Server instance name from Step 2:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_INSTANCE_HERE;Database=BookBuddiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

**Examples**:

If your instance is `(local)`:
```json
"DefaultConnection": "Server=(local);Database=BookBuddiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

If your instance is `.\SQLEXPRESS`:
```json
"DefaultConnection": "Server=.\SQLEXPRESS;Database=BookBuddiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

If your instance is `localhost`:
```json
"DefaultConnection": "Server=localhost;Database=BookBuddiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

**Save the file!**

### Step 5: Install Entity Framework Tools

Install the EF Core CLI tools globally:

```bash
dotnet tool install --global dotnet-ef
```

If already installed, you might see a message saying it's already installed - that's fine!

Verify installation:

```bash
dotnet ef --version
```

You should see the version number (9.0.x or similar).

### Step 6: Create the Database

This is the **magic step** that creates the entire database structure!

Navigate to the Data project folder and run the migration:

```bash
cd BookBuddi.Data
dotnet ef database update --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
```

**What this does**:
- Reads all the migration files in the `BookBuddi.Data/Migrations` folder
- Creates the `BookBuddiDb` database on your SQL Server
- Creates all tables (Books, Authors, Members, Categories, Genres, etc.)
- Adds audit trail columns (CreatedBy, CreatedTime, UpdatedBy, UpdatedTime) to all tables
- Sets up all relationships and constraints
- Populates seed data (10 books, 5 categories, 10 genres, 5 members, sample transactions, etc.)

**Expected output**:
```
Build started...
Build succeeded.
Applying migration '20251029181455_AddAuditTrailColumns'.
Done.
```

**Note**: The `--startup-project` flag is required because the migration is in the Data project but needs configuration from the main BookBuddi.WebApp project.

### Step 7: Verify Database Creation

**Option A: Using SSMS**
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. In Object Explorer, expand **Databases**
4. You should see **BookBuddiDb**
5. Expand it → Expand **Tables**
6. You should see tables like:
   - dbo.AspNetUsers (Admin accounts)
   - dbo.AspNetRoles
   - dbo.Books
   - dbo.Authors
   - dbo.BookAuthors (junction table)
   - dbo.Members
   - dbo.Categories
   - dbo.Genres
   - dbo.BorrowTransactions
   - dbo.Fines
   - dbo.BookRequests
   - dbo.Notifications
   - And more...

**Option B: Using SQL Query**

In SSMS, open a new query window and run:

```sql
USE BookBuddiDb;
GO

-- Check if tables exist
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Check if seed data was inserted
SELECT COUNT(*) AS BookCount FROM Books;
SELECT COUNT(*) AS MemberCount FROM Members;
SELECT COUNT(*) AS CategoryCount FROM Categories;
SELECT COUNT(*) AS GenreCount FROM Genres;
SELECT COUNT(*) AS AuthorCount FROM Authors;

-- Verify audit trail columns were added
SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Books' AND COLUMN_NAME IN ('CreatedBy', 'CreatedTime', 'UpdatedBy', 'UpdatedTime');
```

You should see:
- 12+ tables listed
- 10 books
- 5 members
- 5 categories
- 10 genres
- 10 authors
- 4 audit trail columns for Books table

### Step 8: Build the Project

Navigate back to the solution root and build all projects:

```bash
cd ..
dotnet build
```

**Expected output**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

This builds all four projects:
- BookBuddi.Resources
- BookBuddi.Data
- BookBuddi.Services
- BookBuddi.WebApp (main web application)

### Step 9: Run the Application

Navigate to the main application folder and start it:

```bash
cd BookBuddi.WebApp
dotnet run
```

**Expected output**:
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### Step 10: Test in Browser

1. Open your browser (Chrome or Firefox)
2. Navigate to: `https://localhost:5001`
3. You should see the Book Buddi home page!

**To stop the application**: Press `Ctrl+C` in the terminal

---

## Troubleshooting

### Problem: "Cannot open database 'BookBuddiDb'"

**Solution**: Make sure you ran Step 6 (`dotnet ef database update`)

### Problem: "A network-related or instance-specific error occurred"

**Solutions**:
1. Verify SQL Server service is running (Services → SQL Server)
2. Double-check your connection string in `appsettings.json`
3. Try connecting to SQL Server using SSMS first

### Problem: "Unable to resolve service for type 'ApplicationDbContext'"

**Solution**: Make sure `Program.cs` has this code:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Problem: "dotnet ef command not found"

**Solution**: Install EF Core tools:
```bash
dotnet tool install --global dotnet-ef
```

### Problem: Migration files not found

**Solution**: The migrations should already be in the repository under `BookBuddi.Data/Migrations/` folder. If missing, contact the team lead.

### Problem: "The type or namespace name 'Data' does not exist"

**Solution**: This means the project references aren't restored. Run:
```bash
dotnet restore
```
Then rebuild:
```bash
dotnet build
```

---

## What You Should Have After Setup

After completing all steps, you should have:

✅ All NuGet packages restored
✅ BookBuddiDb database created on your local SQL Server
✅ All tables created with proper relationships
✅ Seed data populated (books, authors, members, etc.)
✅ Application builds successfully
✅ Application runs without errors
✅ Can access the application at https://localhost:5001

---

## Default Credentials

For testing the application:

**Admin Account**:
- **Email**: `admin@bookbuddi.com`
- **Password**: `Admin@123`

**Member Accounts** (Sample users):
- **Email**: `john.doe@example.com` | **Password**: `Password123!`
- **Email**: `jane.smith@example.com` | **Password**: `Password123!`
- **Email**: `robert.j@example.com` | **Password**: `Password123!`
- **Email**: `emily.w@example.com` | **Password**: `Password123!` (Suspended status)
- **Email**: `michael.b@example.com` | **Password**: `Password123!`

---

## Important Notes

### DO NOT Commit These Files

The `.gitignore` file should already exclude these, but be aware:

❌ `appsettings.Development.json` - Contains local settings
❌ `bin/` and `obj/` folders - Build artifacts
❌ `.vs/` and `.vscode/` folders - IDE settings

### DO Commit These Files

✅ All `.cs` files (your code)
✅ `appsettings.json` (with placeholder connection string)
✅ `BookBuddi.Data/Migrations/` folder (database schema)
✅ All `.csproj` files (project configuration)
✅ Solution file (`book-buddi.sln`)

---

## Making Changes to the Database

### If You Add/Modify Entities

If you create new entities or modify existing ones in `BookBuddi.Data/Models/`:

1. **Create a new migration** (from the Data project):
   ```bash
   cd BookBuddi.Data
   dotnet ef migrations add YourMigrationName --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
   ```

2. **Apply the migration**:
   ```bash
   dotnet ef database update --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
   ```

3. **Commit the migration files** (in `BookBuddi.Data/Migrations/`) to Git so team members get the changes

### If Someone Else Added a Migration

When you pull changes from Git and there are new migrations:

1. **Navigate to the Data project**:
   ```bash
   cd BookBuddi.Data
   ```

2. **Update your database**:
   ```bash
   dotnet ef database update --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
   ```

This applies any new migrations to your local database.

---

## Resetting Your Database

If you need to start fresh:

**Option 1: Drop and Recreate (Recommended)**
```bash
cd BookBuddi.Data
dotnet ef database drop --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
dotnet ef database update --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
```

**Option 2: Manual Delete (Using SSMS)**
1. Open SSMS
2. Right-click `BookBuddiDb` → Delete
3. Run:
   ```bash
   cd BookBuddi.Data
   dotnet ef database update --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
   ```

---

## Getting Help

If you encounter issues:

1. Check the **Troubleshooting** section above
2. Check `Documentation/Development-Guides/06-Common-Issues-And-Solutions.md`
3. Ask the team lead
4. Check the project's GitHub Issues page

---

## Quick Reference Commands

```bash
# Navigate to solution root
cd book-buddi

# Restore packages for all projects
dotnet restore

# Build all projects
dotnet build

# Create database (from BookBuddi.Data folder)
cd BookBuddi.Data
dotnet ef database update --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj

# Run application (from BookBuddi.WebApp folder)
cd ../BookBuddi.WebApp
dotnet run

# Create new migration (from BookBuddi.Data folder, after entity changes)
cd BookBuddi.Data
dotnet ef migrations add MigrationName --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj

# Drop database (careful! from BookBuddi.Data folder)
cd BookBuddi.Data
dotnet ef database drop --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj

# Check EF Core version
dotnet ef --version
```

---

## Team Collaboration Tips

1. **Pull before you start working**:
   ```bash
   git pull origin main
   ```

2. **After pulling, update your database**:
   ```bash
   cd BookBuddi.Data
   dotnet ef database update --startup-project ../BookBuddi.WebApp/BookBuddi.WebApp.csproj
   ```

3. **Before committing, build and test**:
   ```bash
   dotnet build
   dotnet run
   ```

4. **Commit migrations along with entity changes**

5. **Communicate database changes** to the team

---

## Contact

If you have questions about setup:
- Team Lead: [Your Name]
- GitHub Repository: https://github.com/Erudi787/book-buddi
- Project Documentation: `Documentation/Development-Guides/`

---

**Last Updated**: 2025-10-30
**Version**: 2.0 - Updated for 3-Tier Architecture Restructure

Good luck! 🚀
