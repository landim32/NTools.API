# NTools

**NTools** is a modular microservice built with **.NET Core** that
provides a collection of tools for various purposes, including file
handling, email sending, document processing, and string utilities. It
is designed to be simple, extensible, and easy to integrate into other
systems.

------------------------------------------------------------------------

## 🚀 Features

-   📧 **MailClient**: Send emails with attachments and templates\
-   📂 **FileClient**: Upload, download, and manage files\
-   📄 **DocumentClient**: Process and generate documents\
-   🔤 **StringClient**: Tools for string manipulation and formatting

------------------------------------------------------------------------

## 🛠️ Technologies Used

  Layer            Technology
  ---------------- --------------------------------------
  Backend          .NET Core (ASP.NET Web API)
  Communication    REST APIs
  Storage          Local/Docker Volume (or S3 optional)
  Authentication   JWT (optional)
  Dev Tools        Docker, Swagger, EF Core

------------------------------------------------------------------------

## 📁 Project Structure

    NTools/
    ├── NTools.API/             # Main API with REST endpoints
    ├── NTools.Domain/          # Business logic and services
    │   ├── MailClient
    │   ├── FileClient
    │   ├── DocumentClient
    │   └── StringClient
    ├── NTools.DTO/             # Data Transfer Objects
    ├── NTools.Tests/           # Unit tests
    └── README.md

------------------------------------------------------------------------

## ⚙️ How to Run

### 1. Clone the repository

``` bash
git clone https://github.com/your-org/ntools.git
cd ntools
```

### 2. Configure environment variables

Edit `.env` or `appsettings.json` to configure: - Email provider
connection - File storage path - Callback URLs and authentication if
needed

### 3. Run with Docker (recommended)

``` bash
docker-compose up --build
```

The app will be available at:\
- Backend API: `http://localhost:5000`

------------------------------------------------------------------------

## 🧪 API Documentation

With the service running, access the Swagger UI to explore endpoints:\
`http://localhost:5000/swagger/index.html`

------------------------------------------------------------------------

## 📦 Integration

You can integrate **NTools** into your project by:\
1. Calling the REST endpoints directly.\
2. Referencing the service layer (`NTools.Domain`) as an internal
library.\
3. Using Docker to keep services decoupled.

------------------------------------------------------------------------

## 🔒 Security

-   JWT tokens for protected endpoints\
-   Optional HTTPS configuration\
-   Best practices for file storage and sensitive data handling

------------------------------------------------------------------------

## 🧩 Future Plans

-   Support for cloud storage providers (AWS S3, Azure Blob)\
-   Dynamic templates for emails and documents\
-   Additional utilities for data manipulation and logging

------------------------------------------------------------------------

## 👨‍💻 Author

Developed by [Rodrigo Landim Carneiro](https://github.com/landim32)

------------------------------------------------------------------------

## 📄 License

MIT License. Free to use and contribute.
