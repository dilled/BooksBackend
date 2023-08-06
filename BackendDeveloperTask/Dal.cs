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
            _configuration = configuration;
            _dbConnectionString = _configuration.GetConnectionString("SQLiteConnectionString");
        }

        public bool IsConnected()
        {
            try
            {
                using (var conn = new SqliteConnection(_dbConnectionString))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            try
            {
                using (var conn = new SqliteConnection(_dbConnectionString))
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM books";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var book = new Book().FromQuery(reader);

                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

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
                        command.CommandText = "SELECT * FROM books WHERE id=@id";
                        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var book = new Book().FromQuery(reader);

                                return book;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return null;
        }

        public async Task<List<Book>> GetBooksByParams(string? author, int? year, string? publisher)
        {
            List<Book> books = new List<Book>();
            try
            {
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

                        command.CommandText = query;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var book = new Book().FromQuery(reader);
                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

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
                        command.CommandText = "DELETE FROM books WHERE id=@id";
                        command.Parameters.Add("@id", SqliteType.Integer).Value = id;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
