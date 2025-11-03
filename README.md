# Gift of the Givers Foundation Web Application

**Module:** Applied Programming (APPR6312)
**Assessment:** POE Part 2

This repository contains the source code for the Gift of the Givers Foundation web application, a comprehensive C# ASP.NET Core MVC project designed to serve as a centralized hub for managing disaster relief efforts.

---

## Team Members (RST Innovations)

| Name                | Student Number | Role                |
| ------------------- | -------------- | ------------------- |
| Tokollo Will Nonyane| ST10296818     | Lead Developer      |
| Sagwadi Mashimbye   | ST10168528     | Database Specialist |
| Rinae Carol Magadagela| ST10361117     | UI/UX Designer      |

---

## Features Implemented

This prototype includes the following key features as per the assignment requirements:

* **Secure User Authentication:** Users can register for an account, log in, and manage their profiles. Role-based authorization is implemented to distinguish between General Users, Volunteers, and Administrators.
* **Disaster Incident Reporting:** Logged-in users can submit reports for new disaster incidents, including details like location, description, and images.
* **Admin Incident Management:** An admin panel allows for the review, approval, editing, and deletion of submitted incidents. Admins can also add logistical details and specific volunteer tasks to approved projects.
* **Volunteer Management System:** A full workflow for volunteer engagement:
    * Users can apply to become a volunteer.
    * Admins can approve applications, which upgrades the user's role.
    * Approved volunteers can browse active projects and sign up for specific tasks.
* **Resource Donation System:** A functional donation page where users can contribute monetarily or donate specific resources to relief efforts. The system tracks these donations.

---

## Technology Stack

* **Framework:** ASP.NET Core MVC (.NET 8/9)
* **Language:** C#
* **Database:** Azure SQL Database with Entity Framework Core
* **Authentication:** ASP.NET Core Identity
* **Version Control:** Git & Azure Repos
* **CI/CD:** Azure Pipelines

---

## Setup and Installation

To run this project locally:

1.  Clone the repository.
2.  Update the `DefaultConnection` string in the `appsettings.json` file with your SQL Server details.
3.  Open the Package Manager Console and run `Update-Database` to apply the migrations.
4.  Run the project. The application will seed the necessary roles and sample data on first launch.
