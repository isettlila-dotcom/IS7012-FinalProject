
# Quick Start

## Auth

### Request these details
CLERK_CLIENT_ID=
CLERK_CLIENT_SECRET=
CLERK_AUTHORITY=

### Set up secrets
dotnet user-secrets init

dotnet user-secrets set "Clerk:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Clerk:ClientSecret" "YOUR_CLIENT_SECRET"
dotnet user-secrets set "Clerk:Authority" "https://YOUR-CLERK-DOMAIN"

dotnet user-secrets list

### Download OIDC
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect --version 10.0.0

### Run
dotnet build
dotnet run --launch-profile https

## Data

### Download
dotnet tool install --global dotnet-ef

### Load data and migrations
dotnet ef database update

# UCIS Payroll Management System

UCIS is a secure payroll management application built with ASP.NET Core Razor Pages. The system supports payroll preparation, review, approval, period management, reporting, and role-based access control.

## Features

- Secure authentication using Clerk and OpenID Connect
- Role-based authorization
- Payroll record creation and editing
- Draft, Submitted, Returned, and Approved workflow states
- Payroll review and approval
- Self-approval prevention
- Payroll period management and closure
- Payroll statistics and reporting
- Employee management
- Friendly Access Denied handling
- Responsive Bootstrap-based user interface

## Technology Stack

- ASP.NET Core Razor Pages
- .NET 10
- Entity Framework Core
- SQLite
- Clerk
- OpenID Connect
- Bootstrap 5
- C#
- Razor / HTML / CSS

## Authentication

Authentication is handled through Clerk using OpenID Connect.

The application uses:

- Cookie authentication for the local application session
- OpenID Connect for Clerk authentication
- Clerk `sub` claims for user identity
- Clerk email claims to map authenticated users to employees in the application database

Application roles are loaded from the employee's associated job role.

## Role-Based Access Control

The application currently supports the following roles:

| Role | Payroll Records | Statistics | Payroll Review | Payroll Periods | Create/Edit Payroll |
|---|---:|---:|---:|---:|---:|
| Payroll Specialist | Yes | Yes | No | No | Yes |
| Payroll Team Lead | Yes | Yes | Yes | Yes | No |
| Non-Payroll User | No | No | No | No | No |

Unauthorized users are redirected to a custom Access Denied page.

## Payroll Workflow

Payroll records move through the following workflow:

```text
Draft
  |
  v
Submitted
  |
  +----> Returned
  |         |
  |         v
  |      Edited
  |         |
  |         v
  |      Resubmitted
  |
  v
Approved
```

### Payroll Specialist

Payroll Specialists can:

- Create payroll records
- Edit Draft records
- Edit Returned records
- Delete Draft or Returned records
- Submit Draft or Returned records for review

### Payroll Team Lead

Payroll Team Leads can:

- View submitted payroll records
- Approve payroll records
- Return payroll records for correction
- Manage payroll periods
- Close completed payroll periods

A Payroll Team Lead cannot approve a payroll record they originally prepared.

## Payroll Period Rules

A payroll period cannot be closed until:

- A payroll record exists for every active employee
- Every payroll record in the period has been approved

Once a payroll period is closed:

- New payroll records cannot be created for that period
- Existing payroll records cannot be edited
- Existing payroll records cannot be deleted
- Records cannot be submitted
- Records cannot be approved or returned

## Database

The application uses Entity Framework Core with SQLite.

The main entities include:

- Employee
- Department
- JobRole
- SalaryPackage
- PayrollPeriod
- PayrollRecord

Payroll records are related to:

- Employees
- Payroll periods
- Salary packages

The database also enforces one payroll record per employee per payroll period.

## Authentication Configuration

Clerk credentials should be stored using .NET User Secrets.

Initialize User Secrets:

```bash
dotnet user-secrets init
```

Set the Clerk configuration:

```bash
dotnet user-secrets set "Clerk:ClientId" "YOUR_CLIENT_ID"

dotnet user-secrets set "Clerk:ClientSecret" "YOUR_CLIENT_SECRET"

dotnet user-secrets set "Clerk:Authority" "https://YOUR-CLERK-DOMAIN"
```

Verify the stored secrets:

```bash
dotnet user-secrets list
```

The Clerk sign-up URL is configured in `appsettings.json`.

Example:

```json
{
  "Clerk": {
    "SignUpUrl": "https://YOUR-CLERK-DOMAIN/sign-up"
  }
}
```

## Database Setup

Install the Entity Framework Core CLI if necessary:

```bash
dotnet tool install --global dotnet-ef
```

Apply migrations:

```bash
dotnet ef database update
```

## Running the Application

Restore dependencies:

```bash
dotnet restore
```

Build the application:

```bash
dotnet build
```

Run using the HTTPS launch profile:

```bash
dotnet run --launch-profile https
```

The development application runs at:

```text
https://localhost:7219
```

## Test Users

Role assignment is based on the authenticated user's email address.

The Clerk email must match an `Employee.Email` value in the application database.

Example mapping:

```text
Clerk User
    |
    v
Email Claim
    |
    v
Employee.Email
    |
    v
Employee.JobRole
    |
    v
AppRole Claim
```

Recommended test identities:

- Payroll Specialist
- Payroll Team Lead
- Non-payroll employee

Use email addresses you control when testing Clerk verification.

## Project Structure

```text
IS7012-FinalProject
|
+-- Data
|   +-- ApplicationDbContext.cs
|
+-- Models
|   +-- Employee.cs
|   +-- Department.cs
|   +-- JobRole.cs
|   +-- SalaryPackage.cs
|   +-- PayrollPeriod.cs
|   +-- PayrollRecord.cs
|   +-- PayrollStatus.cs
|
+-- Pages
|   +-- Employees
|   +-- PayrollRecords
|   +-- PayrollReview
|   +-- PayrollPeriods
|   +-- PayrollStatistics
|   +-- AccessDenied.cshtml
|
+-- Services
|   +-- AppRoleClaimsTransformation.cs
|
+-- wwwroot
|   +-- css
|       +-- site.css
|
+-- Program.cs
+-- appsettings.json
```

## Security Controls

The application includes several security controls:

- Authenticated access to protected pages
- Role-based authorization policies
- Server-side authorization enforcement
- Specialist-only payroll preparation actions
- Team Lead-only payroll review actions
- Team Lead-only payroll period management
- Self-approval prevention
- Closed-period modification protection
- Custom unauthorized-access handling

UI visibility is used for convenience, while authorization is enforced server-side.

## Project Purpose

This application was developed as the final project for IS7012.

The project demonstrates:

- ASP.NET Core web development
- Razor Pages
- Entity Framework Core
- Relational data modeling
- Authentication
- Authorization
- Business workflow enforcement
- Secure application design
- Bootstrap UI development