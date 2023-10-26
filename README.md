# BooksBackend

A simple web backend that stores information about a book collection.

Local Server is listening on http://localhost:9000

## Prerequisites

Before proceeding, ensure you have the following installed on your machine:

1. [.NET 7 SDK](https://dotnet.microsoft.com/download) - Required to build and run the .NET Core application.
2. [Git](https://git-scm.com/downloads) - Required to clone the repository from GitHub.

## Cloning the Repository

Open a terminal or command prompt and run the following command to clone the repository:

```bash
git clone https://github.com/dilled/BooksBackend.git

## Starting the Backend

Open/Run solution in Visual Studio 2022

1. Open a terminal or command prompt. 
2. Navigate to the root folder of the cloned repository using the `cd` command:
cd BooksBackend/BackendDeveloperTask
dotnet run

## Running Tests

To run tests, you can use the .NET CLI. Or Visual Studio, right click on solution and Run Tests.

1. Open a terminal or command prompt.
2. Navigate to the root folder of the cloned repository using the `cd` command (if you're not already there):
cd BooksBackend/BooksBackendTests
dotnet test
