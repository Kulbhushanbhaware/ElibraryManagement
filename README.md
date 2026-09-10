📚 E-Library Management System
A full-featured Library Management System built with ASP.NET Core MVC, using Code-First Entity Framework Core approach with ASP.NET Core Identity for authentication and role-based access.

## 🚀 Tech Stack
- .NET 8
- ASP.NET Core MVC
- Entity Framework Core (Code-First Approach)
- ASP.NET Core Identity (Authentication & Authorization via Areas)
- SQL Server
- Bootstrap (UI)

## ✨ Features
- User Authentication — Secure login/register system using ASP.NET Core Identity
- Book Management — Add, edit, delete, and browse books in the library catalog
- Author & Publisher Management — Maintain author and publisher records linked to books
- Member Management — Register and manage library members
- Book Issue & Return — Issue books to members, track due dates, and process returns
- My Issues Dashboard — Members can view their currently issued books and return history
- Role-Based Access — Separate views/actions for Admin and General users

## 🏗️ Architecture
This project follows the Code-First approach with EF Core Migrations — database schema is generated directly from C# model classes (`Book`, `Author`, `Publisher`, `Member`, `BookIssue`), making the database easy to version and evolve alongside the codebase.

Authentication is implemented using ASP.NET Core Identity, scaffolded into a dedicated Area (`Areas/Identity`) to keep auth-related pages (Login, Register, Logout) cleanly separated from core application logic.

## 📂 Project Structure

ASPIdentityApp/
├── Areas/Identity      → Authentication (Login, Register, Logout)
├── Controllers         → Books, Authors, Publishers, Members, BookIssues
├── Models              → Book, Author, Publisher, Member, BookIssue
├── Migrations          → EF Core Code-First Migrations
├── Views               → Razor views for each module
└── wwwroot             → Static files (CSS, JS, Images)


## ⚙️ Getting Started
1. Clone the repository
2. Update the connection string in `appsettings.json` with your SQL Server details
3. Run migrations: `dotnet ef database update`
4. Run the project: `dotnet run`

## 📌 Note
Connection strings in `appsettings.json` are set to placeholder values for security. Replace them with your own SQL Server credentials before running locally.
