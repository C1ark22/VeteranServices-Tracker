# Veteran Services Tracker

The **Veteran Services Tracker** is a capstone project developed during the Microsoft Software & Systems Academy (MSSA) Cloud Application Development program.

The application is designed as a prototype for a university Veteran Services office to help track students who visit and use a Veterans Corner.

The project started as a local .NET MAUI check-in application and was gradually expanded into a cloud-connected application using an ASP.NET Core Web API, Entity Framework Core, Azure SQL, Azure App Service, authentication, monitoring, and CI/CD.

> This application is a capstone prototype and does not connect to an actual university student information system. Test and mock student information is used during development.

---

## Project Purpose

The goal of this project is to provide a simple way for students to check into a university Veterans Corner while also giving authorized staff a dashboard for viewing visitor activity.

Students can enter their information and complete a check-in without needing a staff account.

Authorized staff members can log in to view check-in history, visitor totals, and student status information.

Managers have additional permissions that allow them to create Staff accounts.

---

## Main Features

### Student Check-In

Students can:

- Enter a Student ID
- Enter their full name
- Select their status
- Read the Veterans Corner Code of Conduct
- Confirm that the Code of Conduct has been read
- Submit a check-in
- Receive confirmation after a successful check-in

Available student statuses include:

- Veteran
- Active Duty
- Reserve
- Dependent
- Civilian

The application validates the student's information before allowing the check-in to be completed.

---

### Staff Dashboard

Authorized Staff and Managers can access a dashboard that displays:

- Total check-ins
- Veteran check-ins
- Active Duty check-ins
- Reserve check-ins
- Dependent check-ins
- Civilian check-ins
- Student ID
- Student name
- Student status
- Check-in date and time

Check-ins are sorted so the **most recent check-in appears first**.

The dashboard can also filter records by:

- Today
- Last 7 Days
- Last 30 Days
- This Month
- Previous Month
- All Check-Ins

---

### Staff Authentication

The original version of the application used a hardcoded Staff username and password.

The current version uses database-backed authentication.

Staff credentials are sent to the ASP.NET Core API, where the password is verified against a stored password hash.

After a successful login, the API creates a **JSON Web Token (JWT)** that is returned to the MAUI application.

The token is stored using .NET MAUI `SecureStorage` and is sent with protected API requests.

The application currently supports two roles:

#### Manager

A Manager can:

- Log in
- View the Staff Dashboard
- View historical check-ins
- Create Staff accounts

#### Staff

A Staff member can:

- Log in
- View the Staff Dashboard
- View historical check-ins

Staff accounts cannot create additional accounts.

---

### Manager Account Management

The Manager can create new Staff accounts from the application.

When a Staff account is created:

1. The Manager must be authenticated.
2. The Manager's JWT is sent with the API request.
3. The API verifies that the user has the `Manager` role.
4. The Staff password is hashed.
5. The new Staff account is stored in Azure SQL.

Passwords are not stored in plain text.

---

## Application Architecture

The current application uses the following architecture:

```text
                    STUDENT
                       |
                       v
                 .NET MAUI App
                       |
                POST /api/CheckIns
                       |
                       v
              ASP.NET Core Web API
                       |
                Entity Framework Core
                       |
                       v
                   Azure SQL


                    STAFF
                       |
                       v
                 .NET MAUI App
                       |
                POST /api/Auth/login
                       |
                       v
              ASP.NET Core Web API
                       |
                JWT Authentication
                       |
                       v
                   Azure SQL


              STAFF / MANAGER
                       |
                       v
                 Staff Dashboard
                       |
                GET /api/CheckIns
                       |
                  JWT Required
                       |
                       v
              ASP.NET Core Web API
                       |
                Entity Framework Core
                       |
                       v
                   Azure SQL
```

---

## Cloud Architecture

The backend API is hosted using **Azure App Service**.

Student and Staff information is stored using **Azure SQL Database**.

The MAUI application communicates with the API using HTTP and JSON.

```text
.NET MAUI
    |
    | HTTP / JSON
    v
Azure App Service
    |
    v
ASP.NET Core Web API
    |
    v
Entity Framework Core
    |
    v
Azure SQL Database
```

---

## Technologies Used

### Application

- C#
- .NET MAUI
- XAML
- .NET SecureStorage
- HttpClient
- JSON
- LINQ
- Async / Await

### Backend

- ASP.NET Core Web API
- REST APIs
- Dependency Injection
- JWT Authentication
- Role-Based Authorization
- Password Hashing

### Database

- Entity Framework Core
- EF Core Migrations
- SQL
- Azure SQL Database

### Azure

- Azure App Service
- Azure SQL Database
- Application Insights

### DevOps

- Git
- GitHub
- GitHub Actions
- CI/CD

---

## API Endpoints

### Create Student Check-In

```http
POST /api/CheckIns
```

Creates a student check-in.

This endpoint remains available to the student check-in application without requiring a Staff login.

---

### View Student Check-Ins

```http
GET /api/CheckIns
```

Returns student check-in records.

This endpoint requires authentication and is limited to:

- Manager
- Staff

---

### Staff Login

```http
POST /api/Auth/login
```

Authenticates Staff and Manager accounts and returns a JWT.

---

### Create Staff Account

```http
POST /api/Staff
```

Creates a new Staff account.

This endpoint requires the:

```text
Manager
```

role.

---

## Database

The application currently uses two main tables.

### StudentCheckIns

Stores student check-in information including:

```text
Id
StudentId
FullName
Status
CheckInTime
```

### StaffUsers

Stores Staff account information including:

```text
Id
Username
PasswordHash
Role
IsActive
CreatedAt
```

Staff passwords are stored as hashes rather than plain-text passwords.

---

## Entity Framework Core

Entity Framework Core is used to communicate between the ASP.NET Core API and the SQL database.

The project uses:

- `DbContext`
- `DbSet`
- LINQ
- Async database operations
- EF Core migrations

The database schema was created and updated using migrations.

Examples include:

```text
InitialCreate
AddStaffUsers
```

---

## Security

Several security concepts were implemented during development.

### Password Hashing

Staff passwords are hashed before being stored in the database.

The application uses:

```csharp
IPasswordHasher<StaffUser>
```

to hash and verify Staff passwords.

---

### JWT Authentication

After a successful Staff login, the API creates a JSON Web Token.

The token contains information such as the user's role.

Protected endpoints verify the token before allowing access.

For example:

```csharp
[Authorize(Roles = "Manager,Staff")]
```

protects student check-in history.

Manager-only functionality uses:

```csharp
[Authorize(Roles = "Manager")]
```

---

### MAUI SecureStorage

The MAUI application stores the authentication token using:

```csharp
SecureStorage
```

rather than storing the JWT as a normal application setting.

The token and Staff information are removed when the user signs out.

---

## Local Encryption Prototype

Before moving the application to a cloud database, an earlier version stored check-in records locally.

That version used:

- CSV storage
- JSON serialization
- AES-GCM encryption
- .NET MAUI SecureStorage for the encryption key

This version helped demonstrate local data security concepts before the application was expanded into a cloud architecture.

The current cloud version uses Azure SQL as the primary source of check-in information.

---

## Application Insights

Azure Application Insights is enabled for the deployed ASP.NET Core API.

Application Insights is used to monitor:

- API requests
- Request count
- Failed requests
- Response times
- API operations
- Dependencies

Examples of monitored operations include:

```text
POST Auth/Login
POST CheckIns/CreateCheckIn
GET CheckIns/GetCheckIns
```

This allows the application to be monitored after deployment instead of relying only on local debugging.

---

## CI/CD

The project uses **GitHub Actions** for Continuous Integration and Continuous Deployment.

When API changes are pushed to the `main` branch, GitHub Actions automatically:

```text
Checkout repository
        |
        v
Setup .NET
        |
        v
Restore API packages
        |
        v
Build API
        |
        v
Publish API
        |
        v
Deploy to Azure App Service
```

Only the ASP.NET Core API project is deployed through the workflow.

The MAUI application is not automatically deployed.

Database migrations are also not automatically executed through the pipeline.

---

## Project Structure

```text
VeteranServices-Tracker
|
|-- SFSU_VeteranServices_Tracker
|   |
|   |-- Model
|   |   |-- StudentCheckIn.cs
|   |   |-- LoginRequest.cs
|   |   |-- LoginResponse.cs
|   |   |-- CreateStaffRequest.cs
|   |
|   |-- Services
|   |   |-- ApiService.cs
|   |   |-- EncryptionService.cs
|   |
|   |-- View
|   |   |-- MainPage.xaml
|   |   |-- MainPage.xaml.cs
|   |   |-- LoginPage.xaml
|   |   |-- LoginPage.xaml.cs
|   |   |-- StaffPage.xaml
|   |   |-- StaffPage.xaml.cs
|   |   |-- ManageStaffPage.xaml
|   |   |-- ManageStaffPage.xaml.cs
|   |
|   |-- App.xaml
|   |-- AppShell.xaml
|   |-- MauiProgram.cs
|
|-- SFSU_VeteranServices_Tracker.Api
|   |
|   |-- Controllers
|   |   |-- AuthController.cs
|   |   |-- CheckInsController.cs
|   |   |-- StaffController.cs
|   |
|   |-- Data
|   |   |-- VeteranServicesContext.cs
|   |
|   |-- Models
|   |   |-- StudentCheckIn.cs
|   |   |-- StaffUser.cs
|   |   |-- LoginRequest.cs
|   |   |-- LoginResponse.cs
|   |   |-- CreateStaffRequest.cs
|   |
|   |-- Migrations
|   |
|   |-- Program.cs
|   |-- appsettings.json
|
|-- .github
|   |
|   |-- workflows
|       |-- api-cicd.yml
|
|-- VeteranServices-tracker.slnx
|
|-- README.md
```

---

## Project Evolution

The Veteran Services Tracker was developed in several stages.

### Version 1 - Local MAUI Application

The first version focused on:

- Student form
- Input validation
- Status selection
- Code of Conduct
- Local check-in records

### Version 2 - Local Data Security

The application was expanded with:

- CSV storage
- JSON serialization
- AES-GCM encryption
- SecureStorage
- Staff dashboard

### Version 3 - Web API and Database

The next version introduced:

- ASP.NET Core Web API
- REST endpoints
- Entity Framework Core
- SQL database
- GET and POST operations

### Version 4 - Azure

The backend was moved to:

- Azure App Service
- Azure SQL Database

The MAUI application then communicated with the deployed API instead of localhost.

### Version 5 - Authentication and Authorization

The Staff system was expanded with:

- Database-backed Staff accounts
- Password hashing
- JWT authentication
- Manager / Staff roles
- Manager-only account creation
- Protected student records

### Version 6 - Monitoring and DevOps

The final cloud architecture added:

- Application Insights
- GitHub Actions
- Automated API deployment
- CI/CD

---

## Testing

The application was tested for:

- Required student fields
- Student ID validation
- Code of Conduct acknowledgment
- Successful student check-in
- Azure SQL persistence
- Correct Staff and Manager login
- Incorrect password rejection
- Manager account permissions
- Staff account permissions
- Duplicate Staff username prevention
- Staff sign-out
- Dashboard status totals
- Dashboard date filtering
- Newest check-in ordering
- Unauthorized API access
- Application Insights telemetry
- GitHub Actions deployment

The application passed the final end-to-end testing workflow.

---

## Setup Requirements

Development was performed using:

- Visual Studio 2022 or newer
- .NET SDK
- .NET MAUI workload
- Git
- Azure subscription for cloud resources

When installing Visual Studio, make sure the following workload is installed:

```text
.NET Multi-platform App UI development
```

---

## Running the Application

### 1. Clone the repository

```bash
git clone https://github.com/C1ark22/VeteranServices-Tracker.git
```

### 2. Open the solution

Open:

```text
VeteranServices-tracker.slnx
```

in Visual Studio.

### 3. Restore dependencies

Visual Studio should restore the required NuGet packages automatically.

### 4. Select the MAUI application

Set:

```text
SFSU_VeteranServices_Tracker
```

as the startup project.

### 5. Select a target platform

For example:

```text
Windows Machine
```

### 6. Build the solution

Use:

```text
Build → Rebuild Solution
```

### 7. Run the application

Start the MAUI application and complete a student check-in or Staff login.

---

## Configuration and Secrets

Sensitive configuration is intentionally not stored in this repository.

Examples include:

- Azure SQL credentials
- JWT signing key
- Manager bootstrap password
- Azure deployment credentials

Local development secrets are stored using **.NET User Secrets**.

Production configuration is stored using **Azure App Service environment variables and connection strings**.

GitHub deployment credentials are stored using **GitHub Actions Secrets**.

---

## Future Improvements

The current application is a capstone prototype rather than a production university system.

Possible future improvements include:

- Microsoft Entra ID authentication
- University Single Sign-On
- Azure Key Vault
- Managed Identity
- Private Azure networking
- Password reset and change functionality
- Audit logging
- Student search
- Check-in and check-out tracking
- Reports and analytics
- Monthly statistics
- Database pagination
- Server-side filtering
- Automated testing
- Improved responsive UI
- Production security and privacy review

A production deployment involving real university student information would require additional institutional security, privacy, access-control, and compliance review.

---

## MSSA Concepts Demonstrated

This project was created to demonstrate concepts learned during MSSA, including:

- C#
- Object-Oriented Programming
- Classes and objects
- Variables
- Conditional statements
- Loops
- Methods
- Collections
- LINQ
- Async / Await
- Exception handling
- XAML
- .NET MAUI
- REST APIs
- JSON
- ASP.NET Core
- Dependency Injection
- Entity Framework Core
- SQL
- Authentication
- Authorization
- Cloud deployment
- Application monitoring
- Git
- GitHub
- CI/CD

---

## Project Status

| Feature | Status |
|---|---|
| Student Check-In | Complete |
| Staff Dashboard | Complete |
| Historical Check-In Filters | Complete |
| ASP.NET Core API | Complete |
| Entity Framework Core | Complete |
| Azure SQL | Complete |
| Azure App Service | Complete |
| Staff Authentication | Complete |
| Manager / Staff Roles | Complete |
| Application Insights | Complete |
| GitHub Actions CI/CD | Complete |
| Final Testing | Complete |

The technical implementation of the capstone is complete.

Current work is focused on documentation and presentation preparation.
