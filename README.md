# Book Buddi

A comprehensive web-based Library Management and Book Catalogue System built with ASP.NET Core and Razor Pages.

## Overview

Book Buddi is designed for both general users and administrators, providing a complete solution for library operations including book cataloguing, borrowing management, and comprehensive reporting.

## Features

### User Features
- **Book Catalogue**: Browse comprehensive book information with detailed metadata
- **Advanced Search**: Robust search functionality to find books quickly
- **Borrowing System**: Borrow and return books seamlessly
- **Book Requests**: Place requests for titles not currently available
- **Notifications**: Personalized notifications for:
  - Book request updates
  - Due date reminders
  - Overdue return alerts
  - Accessible via dedicated page and dropdown panel

### Admin Features
- **Secure Admin Panel**: Authentication-protected administration interface
- **Catalogue Management**:
  - List all books
  - Add new books
  - Edit book details
  - Delete books from catalogue
- **Reporting Module**:
  - Borrowing activity reports
  - Inventory reports
  - Member reports
  - Book request reports
  - Lost books tracking

## Technology Stack

### Frontend
- **Framework**: ASP.NET Core Razor Pages
- **UI Library**: Razor Component Library

### Backend
- **Language**: C#
- **Framework**: ASP.NET Core

### Database
- **DBMS**: Microsoft SQL Server

### Development Tools
- **IDE**: Visual Studio Code

## System Requirements

### Supported Browsers
- ✓ Google Chrome
- ✓ Mozilla Firefox
- ✗ Safari
- ✗ Internet Explorer
- ✗ Microsoft Edge

### Platform Support
- ✓ Windows Desktop
- ✗ Mobile Responsive (Desktop only)

### Display
- **Resolution**: Responsive design for desktop screens

## Getting Started

### 🚀 Quick Setup for Team Members

👉 **[Team Setup Instructions](Documentation/SETUP-FOR-TEAM.md)** 👈

This guide includes:
- Complete prerequisites checklist
- Step-by-step installation
- Database setup with `dotnet ef database update`
- Troubleshooting common issues
- Team collaboration workflow

### Prerequisites

Before you begin, ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/download) (version 9.0 or later)
- [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition or higher)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) for VS Code

### Quick Installation

1. Clone the repository
```bash
git clone https://github.com/Erudi787/book-buddi.git
cd book-buddi
```

2. Navigate to project folder and restore dependencies
```bash
cd BookBuddi
dotnet restore
```

3. Update database connection string in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=BookBuddiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```
> **Note**: Replace `(local)` with your SQL Server instance name. Common values: `(local)`, `.\SQLEXPRESS`, `localhost`

4. Install EF Core tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

5. Create the database and apply migrations
```bash
dotnet ef database update
```
> This creates the `BookBuddiDb` database with all tables and seed data

6. Run the application
```bash
dotnet run
```

7. Open your browser and navigate to `https://localhost:5001` (or the port shown in the console)

### Default Credentials

**Admin Access** (when implemented):
- Email: `admin@bookbuddi.com`
- Password: `Admin@123`

## Project Structure

```
book-buddi/
├── Pages/              # Razor Pages
├── Models/             # Data models
├── Interfaces/         # Interfaces
├── Data/               # Database context and migrations
├── Documentation/      # Documentation Files
├── Services/           # Business logic services
├── wwwroot/            # Static files (CSS, JS, images)
├── appsettings.json    # Configuration
└── Program.cs          # Application entry point
```

## 🔄 Development Workflow

We follow a **lightweight Gitflow-inspired workflow** for teamwork, accountability, and clean code.

1. **Issues** → Every task/feature/bug should have a GitHub Issue.
2. **Branches** → Branch from `develop`, keep changes focused.
3. **Pull Requests (PRs)** → Open into `develop`. Require at least 1 peer review.
4. **Integration** → Test locally, then merge into `develop`.
5. **Release** → Only merge `develop` → `main` when stable.

---

### 🌱 Branching Strategy

* **main** → production-ready, stable code
* **develop** → active development branch
* **feature/** → new features (`feature/denampo-borrow-book`)
* **fix/** → bug fixes (`fix/denampo-auth-bug`)
* **chore/** → configs, setup, maintenance

```bash
git checkout -b feature/<lastname>-<short-description>
```

**Examples:**

* `feature/denampo-borrow-book`
* `fix/denampo-auth-bug`

**Rules:**

* Use **lowercase** (except names).
* Keep names short and descriptive.
* Use **hyphens (-)**, not spaces.

---

### 📝 Commit Guidelines

Format:

```bash
<prefix>(<scope>): <message> - <name>
```

**Examples:**

* `feat(borrow): implement book borrowing - Denampo`
* `fix(auth): resolve auth JWT bug - Denampo`
* `docs(readme): update commit guidelines - Denampo`

**Rules:**

* Prefix must follow the table below.
* Use **lowercase** (except names).
* Keep messages concise.
* Scope is optional, but recommended.

#### 📌 Commit Prefixes

| Prefix        | Meaning                                          |
| ------------- | ------------------------------------------------ |
| **feat:**     | A new feature                                    |
| **fix:**      | A bug fix                                        |
| **docs:**     | Documentation only changes                       |
| **style:**    | Code style changes (formatting, no logic change) |
| **refactor:** | Refactoring code (not a fix or feature)          |
| **test:**     | Adding or fixing tests                           |
| **chore:**    | Maintenance tasks (build, deps, configs, etc.)   |

---

## 🤝 Contributing & Git Workflow

1. Pull the latest code

   ```bash
   git pull origin main
   ```
2. Create a new branch

   ```bash
   git checkout -b feature/<lastname>-<short-description>
   ```
3. Commit changes using the [Commit Guidelines](#-commit-guidelines).
4. Push your branch

   ```bash
   git push origin feature/<lastname>-<short-description>
   ```
5. Open a Pull Request → target `development`.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

Project Link: [https://github.com/Erudi787/book-buddi](https://github.com/Erudi787/book-buddi)

## Acknowledgments

- ASP.NET Core Team
- Razor Component Library contributors
- All contributors to this project
