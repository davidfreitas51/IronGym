# IronGym

IronGym is a role-based management system with JWT token-based authentication, email verification, and role-based access control via a REST API. It includes user management features and employee management for Admins. The project is covered by Unit Testing using FluentAssertions and FakeItEasy

## Technologies
Some of the technologies adopted in the whole project are:
- C# / .NET
- ASP.NET Core
- API REST
- HTML5 and CSS3
- SQL Server
- EF Core
- Unity Testing

### Patterns:
- MVC for the website
- Clean Architecture for the API
- Unity of works
- Independency Injection
- SOLID and Clean Code principles

### Safety Measures:
- JWT Tokens
- Password Hashing for user credentials
- User accounts encrypted with AES
- Role-Based Authentication and Authorization
- Cookie-Based Sessions

## Installation

1. In your Visual Studio or preferred IDE, pull the repository. 
2. Run the "API" project, inside the "BackEnd" folder.
3. Run the "MVC" project, inside the "FrontEnd" folder.

You can do this by setting up both project as startup

If any doubts, check the demonstration video 

## Usage

IronGym simplifies the management of clients and employees. Here’s how to use it effectively:

1. **Register/Login**: Start by creating a new account or logging in to an existing one. You may also use the default user account if you prefer not to create a personal account.

2. **Email Verification**: IronGym verifies user email addresses via code to validate accounts, ensuring that only legitimate users can access the app.

3. **User Area**: Once logged in, users can view and edit their personal infomation. Employees also have the ability to modify user data.

4. **Employee Area**: Employees can log in to their exclusive area, where they can view, edit, and delete client information. Use the default employee login to access this area.

5. **Manager Area**: Managers have access to all employee functionalities, plus additional permissions to manage CRUD operations on employee data. Use the default manager login to access this area.


## Demonstration

Here is a video demonstration of the application's use: [Watch the video](https://youtu.be/ArwHsVdXbE0)
