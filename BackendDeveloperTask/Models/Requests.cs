using Microsoft.AspNetCore.Mvc;

namespace BackendDeveloperTask.Models
{
    // The class representing the request model for getting books by parameters
    [Serializable]
    public class GetBookRequest
    {
        // The author of the book
        public string? author { get; set; }

        // The year of publication of the book
        public string? year { get; set; }

        // The publisher of the book
        public string? publisher { get; set; }

        // Method to validate the GetBookRequest object's properties
        public bool isValid()
        {
            // Check if the author is not an empty string
            if (author == string.Empty)
            {
                return false;
            }

            // Check if the year is not null and can be parsed to an integer
            if (year != null && !int.TryParse(year?.ToString(), out _))
            {
                return false;
            }

            // Check if the publisher is not an empty string
            if (publisher == string.Empty)
            {
                return false;
            }

            // If all checks pass, the GetBookRequest object is considered valid
            return true;
        }
    }

    // The class representing the response model for getting books by parameters
    [Serializable]
    public class GetBookResponse
    {
        // The list of books retrieved from the database
        public List<Book> books = new List<Book>();
    }

    // The class representing the request model for posting a new book
    [Serializable]
    [BindProperties]
    public class PostBookRequest
    {
        // The title of the book
        public string? title { get; set; }

        // The author of the book
        public string? author { get; set; }

        // The year of publication of the book
        public string? year { get; set; }

        // The publisher of the book
        public string? publisher { get; set; }

        // The description of the book
        public string? description { get; set; }
    }

    // The class representing the response model for posting a new book
    [Serializable]
    public class PostBookResponse
    {
        // The ID of the newly created book
        public int id { get; set; }
    }
}
