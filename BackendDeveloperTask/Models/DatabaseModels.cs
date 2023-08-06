using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using SQLite;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace BackendDeveloperTask.Models
{
    // The base abstract class for items stored in the database
    [Serializable]
    public abstract class DatabaseItem
    {
        // Method to safely read a string value from the SqliteDataReader at the given key
        protected string TryReadString(SqliteDataReader reader, int key)
        {
            try
            {
                // Check if the value is NULL in the database
                if (reader.IsDBNull(key))
                    return null;

                // Read the string value at the specified key
                return reader.GetString(key);
            }
            catch
            {
                // If any exception occurs while reading the value, return null
                return null;
            }
        }
    }

    // The class representing a book entity stored in the database
    [Serializable]
    public class Book : DatabaseItem
    {
        // The ID of the book, marked as a key in the database
        [Key]
        public int? id { get; set; }

        // The title of the book, marked as unique in the database
        [Unique]
        public string? title { get; set; }

        // The author of the book, marked as unique in the database
        [Unique]
        public string? author { get; set; }

        // The year of publication of the book, marked as unique in the database
        [Unique]
        public int? year { get; set; }

        // The publisher of the book
        public string? publisher { get; set; }

        // The description of the book
        public string? description { get; set; }

        // Method to populate the Book object from the data read from the database
        public Book FromQuery(SqliteDataReader reader)
        {
            id = reader.GetInt32(0);
            title = reader.GetString(1);
            author = reader.GetString(2);
            year = reader.GetInt32(3);
            publisher = TryReadString(reader, 4);
            description = TryReadString(reader, 5);
            return this;
        }

        // Method to validate the Book object's properties
        public bool isValid()
        {
            // Check if the title is not null or empty
            if (string.IsNullOrEmpty(title))
            {
                return false;
            }

            // Check if the author is not null or empty
            if (string.IsNullOrEmpty(author))
            {
                return false;
            }

            // Check if the publisher is not an empty string
            if (publisher == string.Empty)
            {
                return false;
            }

            // Check if the year can be parsed to an integer
            if (!int.TryParse(year?.ToString(), out _))
            {
                return false;
            }

            // If all checks pass, the Book object is considered valid
            return true;
        }
    }
}
