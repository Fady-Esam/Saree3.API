namespace Saree3.API.Models
{
    public class APIResponse
    {
        public bool Success { get; set; } // Indicates if the request was successful
        public string ErrMessage { get; set; } // Human-readable message for the client
        public object Data { get; set; } // Generic type to hold any kind of data (e.g., a user object, list, etc.)
        //public List<string> Errors { get; set; } // Optional list of error messages

        //public APIResponse()
        //{
        //    Errors = new List<string>();
        //}
    }
}
