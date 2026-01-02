# Project Management RESTful API

A RESTful API built with **ASP.NET Core** using **Clean Architecture principles**.  
The API manages **Project Managers** and **Developers**, with secure **Authentication and Authorization** implemented using **ASP.NET Core Identity**.

---

## 📌 Overview

This project is designed to demonstrate a scalable and maintainable backend system using Clean Architecture.  
It separates concerns across four layers while enforcing security using Identity-based authentication.

---

## 🏗 Architecture

The solution follows **Clean Architecture**, ensuring that business logic is independent of frameworks and infrastructure concerns.

### Layers

├── API
│ ├── Controllers
│ ├── Middleware
│ ├── Filters
│ └── Program.cs
│
├── Application
│ ├── DTOs
│ ├── Interfaces
│ ├── UseCases / Features
│ ├── Validators
│ └── Mapping
│
├── Domain
│ ├── Entities
│ ├── Enums
│ ├── ValueObjects
│ └── Common
│
└── Infrastructure
├── Persistence
├── Identity
├── Repositories
└── Services


## 🔐 Authentication & Authorization

- Implemented using **ASP.NET Core Identity**
- Supports:
  - User Registration
  - Login
  - Role-based Authorization
- Roles:
  - **ProjectManager**
  - **Developer**

### Authorization
- Role-based access control using `[Authorize(Roles = "...")]`
- JWT Bearer tokens are used for securing API endpoints

---

## 🧑‍💼 User Management

### Project Managers
- Create and manage projects
- Assign developers to projects
- View project progress

### Developers
- View assigned projects
- Update task status
- Access only authorized resources

---

## 🛠 Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- JWT Authentication
- Swagger / OpenAPI
- FluentValidation
- AutoMapper

---

## 🚀 Getting Started

### Prerequisites
- .NET 7 or later
- SQL Server
- Visual Studio 2022+

---

### 🔧 Setup Instructions

1. Clone the repository:
   git clone https://github.com/amosmoyo/ProjectManagement.git
