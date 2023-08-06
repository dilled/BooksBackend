using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using SQLite;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace BackendDeveloperTask.Models
{
    [Serializable]
    public abstract class DatabaseItem
    {
        protected string TryReadString(SqliteDataReader reader, int key)
        {
            try
            {
                if (reader.IsDBNull(key))
                    return null;

                return reader.GetString(key);
            }
            catch
            {
                return null;
            }
        }
    }

    [Serializable]
    public class Book : DatabaseItem
    {
        [Key]
        public int? id { get; set; }

        [Unique]
        public string? title { get; set; }

        [Unique]
        public string? author { get; set; }

        [Unique]
        public int? year { get; set; }

        public string? publisher { get; set; }

        public string? description { get; set; }

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

        public bool isValid()
        {
            if (string.IsNullOrEmpty(title))
            {
                return false;
            }

            if (string.IsNullOrEmpty(author))
            {
                return false;
            }

            if (publisher == string.Empty)
            {
                return false;
            }

            if (!int.TryParse(year?.ToString(), out _))
            {
                return false;
            }

            return true;
        }
    }
}
