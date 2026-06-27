namespace IAD2026.Shared;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, int statusCode = 200) =>
        new() { Success = true, Data = data, StatusCode = statusCode };

    public static ApiResponse<T> ErrorResponse(string errorCode, string message, int statusCode = 500) =>
        new() { Success = false, ErrorCode = errorCode, Message = message, StatusCode = statusCode };
}