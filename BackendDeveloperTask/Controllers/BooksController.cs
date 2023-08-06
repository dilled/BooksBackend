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
            if (_dal == null)
            {
                _dal = new Dal(configuration);
            }
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetBooksByParams([FromQuery]GetBookRequest request)
        {
            try
            {
                GetBookResponse getBookResponse = new GetBookResponse();
                if (request.author == null && request.year == null && request.publisher == null)
                {
                    getBookResponse.books = await _dal.GetAllBooks();

                    if (getBookResponse.books.Count == 0)
                    {
                        return NotFound();
                    }

                    return JsonConvert.SerializeObject(getBookResponse.books);
                }
                else
                {
                    if (!request.isValid())
                    {
                        return BadRequest();
                    }

                    getBookResponse.books = await _dal.GetBooksByParams(request.author, request.year != null ? int.Parse(request.year) : null, request.publisher);

                    return JsonConvert.SerializeObject(getBookResponse.books);
                }
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetBookById(string id)
        {
            try
            {
                if (!int.TryParse(id, out int bookId))
                {
                    return NotFound();
                }
                var book = await _dal.GetBookById(bookId);
                if (book != null)
                {
                    return JsonConvert.SerializeObject(book);
                }
                return NotFound();
            }
            catch
            {
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

                if (book == null)
                {
                    return BadRequest();
                }

                if (!book.isValid())
                {
                    return BadRequest();
                }
                PostBookResponse response = new PostBookResponse { id = await _dal.PostBook(book) };

                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                return BadRequest();
            }
            
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteBook(string id)
        {
            try
            {
                if (!int.TryParse(id, out int bookId))
                {
                    return NotFound();
                }
                if (await _dal.GetBookById(bookId) == null)
                {
                    return NotFound();
                }
                await _dal.DeleteBookById(bookId);
                return NoContent();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
