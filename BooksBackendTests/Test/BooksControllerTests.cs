using BackendDeveloperTask.Models;
using BooksBackendTests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace BooksBackendTests.Test
{
    // The test class for the BooksController
    public class BooksControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public HttpClient _client { get; }
        private readonly DbContext _dbContext;

        // The base URL for the HTTP requests
        private string url = "https://booksbackend.azurewebsites.net/books";

        // Constructor to initialize the test class with the custom web application factory
        public BooksControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            // Initialize the database context to interact with the in-memory SQLite database
            var dbContextFactory = factory.Services.GetRequiredService<IDbContextFactory<BackendDeveloperTask.Data.AppDbContext>>();
            _dbContext = dbContextFactory.CreateDbContext();
            _dbContext.Database.EnsureCreated();
        }

        // Dispose method to clean up the database context after each test
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        // Method to reset the database to its initial state
        public void ResetDatabase()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.Migrate();
        }

        // Method to send a POST request to the backend and return the response
        public async Task<ActionResult<object>> SendPostRequest(string json)
        {
            // Set the 'Content-Type' header to indicate JSON payload
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var res = await _client.PostAsync(url, httpContent);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string>(content);
            }
            else
            {
                // Return the StatusCodeResult for non-successful responses
                return res.StatusCode;
            }
        }

        // Method to send a GET request to the backend with query parameters and return the response
        public async Task<ActionResult<string>> SendGetRequestByParams(string param = null)
        {
            var currentUrl = url;
            if (param != null) currentUrl += "?" + param;
            var res = await _client.GetAsync(currentUrl);
            var content = await res.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<string>(content);
        }

        // Method to send a GET request to the backend by book ID and return the response
        public async Task<ActionResult<object>> SendGetRequestById(string id)
        {
            string param = $"/{id}";
            var res = await _client.GetAsync(url + param);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string>(content);
            }
            else
            {
                // Return the StatusCodeResult for non-successful responses
                return res.StatusCode;
            }
        }

        // Method to send a DELETE request to the backend and return the response
        public async Task<ActionResult<object>> SendDeleteRequest(object id)
        {
            // Send the DELETE request using the HttpClient
            string param = $"/{id}";
            var res = await _client.DeleteAsync(url + param);

            return res.StatusCode;
        }

        // Method to initialize the database with test books
        public async Task InitDBWithBooks()
        {
            foreach (var bookString in JsonStrings.validBooks)
            {
                var response = await SendPostRequest(bookString);
            }
        }

        // Test method for POST requests to create books
        [Fact]
        public async Task TestPostBooks()
        {
            // Arrange
            ResetDatabase();
            int i = 1;
            foreach (var bookString in JsonStrings.validBooks)
            {
                var response = await SendPostRequest(bookString);
                Assert.Equal(JsonConvert.SerializeObject(new PostBookResponse { id = i }), response.Value);
                i++;
            }

            var allBooksResponse = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(allBooksResponse.Value);
            Assert.Equal(4, books.Count);
        }

        // Test method for failing POST requests to create books
        [Fact]
        public async Task TestPostBook_fails()
        {
            // Arrange
            ResetDatabase();

            // Test different scenarios for invalid book data
            var result = await SendPostRequest(JsonStrings.missingTitle);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.missingYear);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            // ... Additional test cases for other invalid book data ...

            // Test successful creation of a valid book
            var passingbook = "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
                "}";
            result = await SendPostRequest(passingbook);
            Assert.Equal(JsonConvert.SerializeObject(new PostBookResponse { id = 1 }), result.Value);

            // Test duplicate book creation
            result = await SendPostRequest(passingbook);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            // ... Additional test cases for other invalid book creations ...

            // Test invalid JSON format
            var invalidJson = "{\"whoops\"}";
            result = await SendPostRequest(invalidJson);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);
        }

        // Test method for getting all books
        [Fact]
        public async Task TestGetAllBooks()
        {
            // Arrange
            ResetDatabase();
            await InitDBWithBooks();

            // Act
            var result = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(result.Value);

            // Assert
            Assert.Equal(4, books.Count);
            // ... Additional assertions for verifying book data ...
        }

        // Test method for getting a book by its ID
        [Fact]
        public async Task TestGetBookById()
        {
            // Arrange
            ResetDatabase();
            await InitDBWithBooks();

            // Act
            var result = await SendGetRequestById("1");
            var book = JsonConvert.DeserializeObject<Book>((string)result.Value);
            // ... Additional assertions for verifying book data ...

            // Test scenario for non-existing book ID
            result = await SendGetRequestById("0");
            Assert.Equal(HttpStatusCode.NotFound, result.Value);
        }

        // Test method for getting books by query parameters
        [Fact]
        public async Task TestGetBooksByParam()
        {
            // Arrange
            ResetDatabase();
            await InitDBWithBooks();

            // Act
            var result = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(4, books.Count);
            // ... Additional test cases for filtering books by different parameters ...
        }

        // Test method for deleting books
        [Fact]
        public async Task TestDeleteBooks()
        {
            // Arrange
            ResetDatabase();
            await InitDBWithBooks();
            var result = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(result.Value);

            // Act & Assert
            for (int i = 1; i < books.Count; i++)
            {
                var res = await SendDeleteRequest(i);
                Assert.Equal(HttpStatusCode.NoContent, res.Value);
            }

            for (int i = 1; i < books.Count; i++)
            {
                var res = await SendDeleteRequest(i);
                Assert.Equal(HttpStatusCode.NotFound, res.Value);
            }

            var invalidRes = await SendDeleteRequest("1.1");
            Assert.Equal(HttpStatusCode.NotFound, invalidRes.Value);

            invalidRes = await SendDeleteRequest("x");
            Assert.Equal(HttpStatusCode.NotFound, invalidRes.Value);
        }
    }
}
