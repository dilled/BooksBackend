using Microsoft.AspNetCore.Mvc;

namespace BackendDeveloperTask.Models
{
    [Serializable]
    public class GetBookRequest
    {
        public string? author { get; set; }
        public string? year { get; set; }
        public string? publisher { get; set; }

        public bool isValid()
        {

            if (author == string.Empty)
            {
                return false;
            }

            if (year != null && !int.TryParse(year?.ToString(), out _))
            {
                return false;
            }

            if (publisher == string.Empty)
            {
                return false;
            }

            return true;
        }
    }

    [Serializable]
    public class GetBookResponse
    {
        public List<Book> books = new List<Book>();
    }

    [Serializable]
    [BindProperties]
    public class PostBookRequest
    {
        public string? title { get; set; }
        public string? author { get; set; }
        public string? year { get; set; }
        public string? publisher { get; set; }
        public string? description { get; set; }
    }

    [Serializable]
    public class PostBookResponse
    { 
        public int id { get; set; }
    }
}
