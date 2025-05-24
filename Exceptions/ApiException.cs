namespace DatingApp_API.Exceptions
{
    public class ApiException(int statusCode, string message, string? stackTrace)
    {
        public int StatusCode { get; set; } = statusCode;
        public string Message { get; set; } = message;
        public string? StackTrace { get; set; } = stackTrace;
    }
}
