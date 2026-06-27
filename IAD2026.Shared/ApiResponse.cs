namespace IAD2026.Shared;

public class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorCode { get; init; }
    public string? Message { get; init; }
    public int StatusCode { get; init; }

    public static ApiResponse<T> Success(T? data = default, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Error(string errorCode, string message, int statusCode = 500)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            Message = message,
            StatusCode = statusCode
        };
    }

    // Optional backward compatibility methods
    public static ApiResponse<T> ErrorResponse(string errorCode, string message, int statusCode = 500)
        => Error(errorCode, message, statusCode);
}