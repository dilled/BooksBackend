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
    public class BooksControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public HttpClient _client { get; }
        private readonly DbContext _dbContext;

        private string url = "http://localhost:9000/books";

        public BooksControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            var dbContextFactory = factory.Services.GetRequiredService<IDbContextFactory<BackendDeveloperTask.Data.AppDbContext>> ();
            _dbContext = dbContextFactory.CreateDbContext();
            _dbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        public void ResetDatabase()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.Migrate();
        }

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

        public async Task<ActionResult<string>> SendGetRequestByParams(string param = null)
        {
            var currentUrl = url;
            if (param!=null) currentUrl += "?" +param;
            var res = await _client.GetAsync(currentUrl);
            var content = await res.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<string>(content);
        }

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

        public async Task<ActionResult<object>> SendDeleteRequest(object id)
        {
            // Send the DELETE request using the HttpClient
            string param = $"/{id}";
            var res = await _client.DeleteAsync(url + param);

            return res.StatusCode;
        }

        public async Task InitDBWithBooks()
        {
            foreach (var bookString in JsonStrings.validBooks)
            {
                var response = await SendPostRequest(bookString);
            }
        }

        [Fact]
        public async Task TestPostBooks()
        {
            // Arrange
            ResetDatabase();
            int i = 1;
            foreach(var bookString in JsonStrings.validBooks)
            {
                var response = await SendPostRequest(bookString);
                Assert.Equal(JsonConvert.SerializeObject(new PostBookResponse { id = i }), response.Value);
                i++;
            }

            var allBooksResponse = await SendGetRequestByParams();

            var books = JsonConvert.DeserializeObject<List<Book>>(allBooksResponse.Value);
            Assert.Equal(4, books.Count);
        }

        [Fact]
        public async Task TestPostBook_fails()
        {
            // Arrange
            ResetDatabase();
            
            var result = await SendPostRequest(JsonStrings.missingTitle);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.missingYear);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.invalidYear);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.emptyAuthor);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.emptyTitle);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.nonIntYear);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.stringYear);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            result = await SendPostRequest(JsonStrings.emptyPublisher);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            var passingbook = "{" +
                "\"title\":\"The Hitchhiker's Guide to the Galaxy\"," +
                "\"author\":\"Douglas Adams\"," +
                "\"year\":1979," +
                "\"publisher\":\"Pan Books\"," +
                "\"description\":\"Originally a radio series\"" +
                "}";
            result = await SendPostRequest(passingbook);
            Assert.Equal(JsonConvert.SerializeObject(new PostBookResponse { id = 1}), result.Value);

            var res = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(res.Value);
            Assert.Equal(1, books.Count);

            // Test duplicates
            result = await SendPostRequest(passingbook);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);
            result = await SendPostRequest(passingbook);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);
            result = await SendPostRequest(passingbook);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            res = await SendGetRequestByParams();
            books = JsonConvert.DeserializeObject<List<Book>>(res.Value);
            Assert.Equal(1, books.Count);

            var invalidJson = "{\"whoops\"}";
            result = await SendPostRequest(invalidJson);
            Assert.Equal(HttpStatusCode.BadRequest, result.Value);

            res = await SendGetRequestByParams();
            books = JsonConvert.DeserializeObject<List<Book>>(res.Value);
            Assert.Equal(1, books.Count);
        }

        [Fact]
        public async Task TestGetAllBooks()
        {
            // Arrange
            ResetDatabase();
            await InitDBWithBooks();
            var result = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(result.Value);

            // Assert
            Assert.Equal(4, books.Count);
            Assert.Equal("Harry Potter and the Philosophers Stone", books[0].title);
            Assert.Equal("J.K.Rowling", books[0].author);
            Assert.Equal(1997, books[0].year);
            Assert.Equal("Bloomsbury (UK)", books[0].publisher);
            Assert.Equal("A book about a wizard boy", books[0].description);
        }

        [Fact]
        public async Task TestGetBookById()
        {
            ResetDatabase();
            await InitDBWithBooks();

            var result = await SendGetRequestById("1");
            var book = JsonConvert.DeserializeObject<Book>((string)result.Value);
            Assert.Equal("Harry Potter and the Philosophers Stone", book.title);
            Assert.Equal("J.K.Rowling", book.author);
            Assert.Equal(1997, book.year);
            Assert.Equal("Bloomsbury (UK)", book.publisher);
            Assert.Equal("A book about a wizard boy", book.description);

            result = await SendGetRequestById("4");
            book = JsonConvert.DeserializeObject<Book>((string)result.Value);
            Assert.Equal("Goosebumps: Beware, the Snowman", book.title);
            Assert.Equal("R.L. Stine", book.author);
            Assert.Equal(1997, book.year);
            Assert.Equal("Scholastic Point", book.publisher);
            Assert.Equal(null, book.description);

            result = await SendGetRequestById("0");
            Assert.Equal(HttpStatusCode.NotFound, result.Value);
        }

        [Fact]
        public async Task TestGetBooksByParam()
        {
            ResetDatabase();
            await InitDBWithBooks();

            var result = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(4, books.Count);

            var req = "author=J.K.Rowling";
            result = await SendGetRequestByParams(req);
            books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(1, books.Count);

            req = "author=J.K.Rowling&year=1";
            result = await SendGetRequestByParams(req);
            books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(JsonConvert.SerializeObject(new GetBookResponse().books), result.Value);
            Assert.Equal(0, books.Count);

            req = "publisher=Otava";
            result = await SendGetRequestByParams(req);
            books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(JsonConvert.SerializeObject(new GetBookResponse().books), result.Value);
            Assert.Equal(0, books.Count);

            req = "publisher=Scholastic Point";
            result = await SendGetRequestByParams(req);
            books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(2, books.Count);

            req = "year=1997";
            result = await SendGetRequestByParams(req);
            books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(3, books.Count);

            req = "year=1979";
            result = await SendGetRequestByParams(req);
            books = JsonConvert.DeserializeObject<List<Book>>(result.Value);
            Assert.Equal(JsonConvert.SerializeObject(new GetBookResponse().books), result.Value);
            Assert.Equal(0, books.Count);
        }

        [Fact]
        public async Task TestDeleteBooks()
        {
            // Arrange
            ResetDatabase();
            await InitDBWithBooks();
            var result = await SendGetRequestByParams();
            var books = JsonConvert.DeserializeObject<List<Book>>(result.Value);

            // Assert
            Assert.Equal(4, books.Count);

            for(int i=1; i < books.Count; i++)
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