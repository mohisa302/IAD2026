using System.Net;

namespace IAD2026.Application.Exceptions;

public class ExternalApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }

    public string? RawRequest { get; set; }
    public string? RawResponse { get; set; }

    public ExternalApiException(string message, HttpStatusCode statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}