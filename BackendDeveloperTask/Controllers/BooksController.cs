using Backend;
using BackendDeveloperTask.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BackendDeveloperTask.Controllers
{
    [ApiController]
    [Route("books")]
    public class BooksController : ControllerBase
    {
        private static Dal _dal;

        public BooksController(IConfiguration configuration)
        {
            // Initialize the Data Access Layer (DAL) if not already done
            if (_dal == null)
            {
                _dal = new Dal(configuration);
            }
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetBooksByParams([FromQuery] GetBookRequest request)
        {
            try
            {
                // Create a response object to hold the results
                GetBookResponse getBookResponse = new GetBookResponse();

                // If no query parameters are provided, return all books
                if (request.author == null && request.year == null && request.publisher == null)
                {
                    getBookResponse.books = await _dal.GetAllBooks();

                    // If no books found, return NotFound status
                    if (getBookResponse.books.Count == 0)
                    {
                        return NotFound();
                    }

                    // Serialize the books list and return as JSON
                    return JsonConvert.SerializeObject(getBookResponse.books);
                }
                else
                {
                    // If query parameters are provided, validate the request
                    if (!request.isValid())
                    {
                        // If the request is not valid, return BadRequest status
                        return BadRequest();
                    }

                    // Get books based on the provided query parameters
                    getBookResponse.books = await _dal.GetBooksByParams(request.author, request.year != null ? int.Parse(request.year) : null, request.publisher);

                    // Serialize the books list and return as JSON
                    return JsonConvert.SerializeObject(getBookResponse.books);
                }
            }
            catch
            {
                // If any exception occurs, return NotFound status
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetBookById(string id)
        {
            try
            {
                // Convert the id parameter to an integer
                if (!int.TryParse(id, out int bookId))
                {
                    // If the id parameter is not a valid integer, return NotFound status
                    return NotFound();
                }

                // Get the book by id from the DAL
                var book = await _dal.GetBookById(bookId);

                // If the book is found, serialize it and return as JSON
                if (book != null)
                {
                    return JsonConvert.SerializeObject(book);
                }

                // If the book is not found, return NotFound status
                return NotFound();
            }
            catch
            {
                // If any exception occurs, return NotFound status
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> PostBook([FromBody] object? request)
        {
            try
            {
                // Deserialize the JSON request into the Book class
                var book = JsonConvert.DeserializeObject<Book>(request?.ToString());

                // If the request is null or not a valid Book object, return BadRequest status
                if (book == null || !book.isValid())
                {
                    return BadRequest();
                }

                // Create a response object and store the newly created book's ID
                PostBookResponse response = new PostBookResponse { id = await _dal.PostBook(book) };

                // Serialize the response and return as JSON
                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                // If any exception occurs, return BadRequest status
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteBook(string id)
        {
            try
            {
                // Convert the id parameter to an integer
                if (!int.TryParse(id, out int bookId))
                {
                    // If the id parameter is not a valid integer, return NotFound status
                    return NotFound();
                }

                // Check if the book exists with the provided id
                if (await _dal.GetBookById(bookId) == null)
                {
                    // If the book does not exist, return NotFound status
                    return NotFound();
                }

                // Delete the book by id
                await _dal.DeleteBookById(bookId);

                // Return NoContent status to indicate successful deletion
                return NoContent();
            }
            catch
            {
                // If any exception occurs, return NotFound status
                return NotFound();
            }
        }
    }
}
