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

### Prerequisites

Before you begin, ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or later recommended)
- [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition or higher)
- [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) for VS Code

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/book-buddi.git
cd book-buddi
```

2. Restore dependencies
```bash
dotnet restore
```

3. Update database connection string in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookBuddi;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

4. Apply database migrations
```bash
dotnet ef database update
```

5. Run the application
```bash
dotnet run
```

6. Open your browser and navigate to `https://localhost:5001` (or the port shown in the console)

## Project Structure

```
book-buddi/
├── Pages/              # Razor Pages
├── Models/             # Data models
├── Data/               # Database context and migrations
├── Services/           # Business logic services
├── wwwroot/            # Static files (CSS, JS, images)
├── appsettings.json    # Configuration
└── Program.cs          # Application entry point
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

Project Link: [https://github.com/yourusername/book-buddi](https://github.com/yourusername/book-buddi)

## Acknowledgments

- ASP.NET Core Team
- Razor Component Library contributors
- All contributors to this project
