namespace IAD2026.Shared;

public class PagedResponse<T>
{
    public bool IsSuccess { get; init; }           
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public string? ErrorCode { get; init; }
    public string? Message { get; init; }
    public int StatusCode { get; init; }

    public static PagedResponse<T> Success(
        IReadOnlyList<T> items,
        int totalCount,
        int page,
        int pageSize,
        int statusCode = 200)
    {
        return new PagedResponse<T>
        {
            IsSuccess = true,
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            StatusCode = statusCode
        };
    }

    public static PagedResponse<T> Error(string errorCode, string message, int statusCode = 500)
    {
        return new PagedResponse<T>
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            Message = message,
            StatusCode = statusCode
        };
    }
}