namespace WebApplication1.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; } // The main data or result (e.g., User object)
        public string Message { get; set; } // Success or error message
        public bool Success { get; set; } // Whether the operation was successful
        public int StatusCode { get; set; } // HTTP status code
    }

}
