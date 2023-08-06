using Microsoft.Data.Sqlite;
using BackendDeveloperTask.Models;

namespace Backend
{
    public class Dal
    {
        private IConfiguration _configuration;
        private string _dbConnectionString;

        public Dal(IConfiguration configuration)
        {
            // Initialize the DAL with the provided configuration and set the database connection string
            _configuration = configuration;
            _dbConnectionString = _configuration.GetConnectionString("SQLiteConnectionString");
        }

        public async Task<List<Book>> GetAllBooks()
        {
            // Get all books from the database
            List<Book> books = new List<Book>();
            try
            {
                using (var conn = new SqliteConnection(_dbConnectionString))
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        // Execute the SQL query to select all books from the "books" table
                        command.CommandText = "SELECT * FROM books";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a new Book object from the data read from the database
                                var book = new Book().FromQuery(reader);

                                // Add the book to the list of books
                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch
            {
                // If an exception occurs, throw it to be handled by the caller
                throw;
            }

            // Return the list of books
            return books;
        }

        public async Task<Book> GetBookById(int id)
        {
            try
            {
                using (var conn = new SqliteConnection(_dbConnectionString))
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        // Execute the SQL query to select a book by its ID from the "books" table
                        command.CommandText = "SELECT * FROM books WHERE id=@id";
                        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a new Book object from the data read from the database
                                var book = new Book().FromQuery(reader);

                                // Return the book found with the provided ID
                                return book;
                            }
                        }
                    }
                }
            }
            catch
            {
                // If an exception occurs, throw it to be handled by the caller
                throw;
            }

            // Return null if no book is found with the provided ID
            return null;
        }

        public async Task<List<Book>> GetBooksByParams(string? author, int? year, string? publisher)
        {
            // Get books from the database based on the provided query parameters
            List<Book> books = new List<Book>();
            try
            {
                // Create the base SQL query
                var query = "SELECT * FROM books WHERE 1=1 ";

                using (var conn = new SqliteConnection(_dbConnectionString))
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        // Use parameterized queries to add the conditions based on the provided parameters
                        if (author != null)
                        {
                            query += " AND author = @author";
                            command.Parameters.Add(new SqliteParameter("@author", author));
                        }

                        if (year != null)
                        {
                            query += " AND year = @year";
                            command.Parameters.Add(new SqliteParameter("@year", year));
                        }

                        if (publisher != null)
                        {
                            query += " AND publisher = @publisher";
                            command.Parameters.Add(new SqliteParameter("@publisher", publisher));
                        }

                        // Set the command text to the final query with the added conditions
                        command.CommandText = query;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a new Book object from the data read from the database
                                var book = new Book().FromQuery(reader);

                                // Add the book to the list of books
                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch
            {
                // If an exception occurs, throw it to be handled by the caller
                throw;
            }

            // Return the list of books matching the query parameters
            return books;
        }

        public async Task<int> PostBook(Book book)
        {
            try
            {
                using (var conn = new SqliteConnection(_dbConnectionString))
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        // Execute the SQL query to insert a new book into the "books" table
                        command.CommandText = "INSERT INTO books (title, author, year, publisher, description) " +
                            "VALUES (@title, @author, @year, @publisher, @description)";
                        command.Parameters.Add("@title", SqliteType.Text).Value = book.title;
                        command.Parameters.Add("@author", SqliteType.Text).Value = book.author;
                        command.Parameters.Add("@year", SqliteType.Integer).Value = book.year;
                        command.Parameters.Add("@publisher", SqliteType.Text).Value = book.publisher != null ? book.publisher : DBNull.Value;
                        command.Parameters.Add("@description", SqliteType.Text).Value = book.description != null ? book.description : DBNull.Value;

                        await command.ExecuteNonQueryAsync();
                    }

                    // Get the last inserted ID
                    using (var getIdCommand = conn.CreateCommand())
                    {
                        getIdCommand.CommandText = "SELECT last_insert_rowid()";
                        var newId = await getIdCommand.ExecuteScalarAsync();
                        return Convert.ToInt32(newId);
                    }
                }
            }
            catch
            {
                // If an exception occurs, throw it to be handled by the caller
                throw;
            }
        }

        public async Task DeleteBookById(int id)
        {
            try
            {
                using (var conn = new SqliteConnection(_dbConnectionString))
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        // Execute the SQL query to delete a book by its ID from the "books" table
                        command.CommandText = "DELETE FROM books WHERE id=@id";
                        command.Parameters.Add("@id", SqliteType.Integer).Value = id;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
                // If an exception occurs, throw it to be handled by the caller
                throw;
            }
        }
    }
}
