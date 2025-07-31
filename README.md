# SmartGarage - Vehicle Service Management System

A comprehensive ASP.NET Core web application for managing vehicle service operations, built with modern development practices and a clean architecture approach.

## Project Overview

SmartGarage is a professional vehicle service management system that allows administrators, mechanics, and customers to manage service operations efficiently.

## 🏗️ Architecture

The project follows a clean architecture pattern with separate projects

- **SmartGarage.Domain** - Domain entities and business logic
- **SmartGarage.Razor** - Web UI using ASP.NET Core Razor Pages
- **SmartGarage.Api** - RESTful API for external integrations
- **SmartGarage.Test** - Unit tests for business logic

## 📋 Features

### 🔐 Authentication & Authorization
- Custom session-based authentication system
- Role-based access control (Admin, Mechanic, Customer)
- Secure login/logout functionality
- Automatic user session management

### 👥 User Management
- **Admin Dashboard**: Full system access, customer management, service oversight
- **Mechanic Dashboard**: Service record management, vehicle maintenance tracking
- **Customer Dashboard**: Personal vehicle and service history

### 🔌 API Integration
- RESTful API endpoints for customer data

### 🧪 Testing
- Unit tests for business logic
- Entity Framework In-Memory database for testing

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- MySQL Server
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**

2. **Database Setup**
   - Install MySQL Server
   - Create a database named `WorkShopDBProject`

3. **Run Database Migrations**
   ```bash
   cd SmartGarage.Razor
   dotnet ef database update
   ```

4. **Build and Run**
   ```bash
   dotnet build
   dotnet run
   ```

5. **Access the Application**
   - Web UI: http://localhost:5159
   - API Documentation: http://localhost:5000/swagger

## 👨‍💻 Author

**Student Number**: 73965  
**Course**: Object-Oriented Programming  
