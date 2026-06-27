namespace IAD2026.Shared;

public static class ErrorCodes
{
    // General Errors
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";

    // External API Errors
    public const string ExternalApiError = "EXTERNAL_API_ERROR";
    public const string ExternalBadRequest = "EXTERNAL_BAD_REQUEST";
    public const string ExternalUnauthorized = "EXTERNAL_UNAUTHORIZED";
    public const string ExternalForbidden = "EXTERNAL_FORBIDDEN";
    public const string ExternalNotFound = "EXTERNAL_NOT_FOUND";
    public const string ExternalRateLimited = "EXTERNAL_RATE_LIMITED";
    public const string ExternalServerError = "EXTERNAL_SERVER_ERROR";
    public const string ExternalBadGateway = "EXTERNAL_BAD_GATEWAY";
    public const string ExternalServiceUnavailable = "EXTERNAL_SERVICE_UNAVAILABLE";
    public const string ExternalGatewayTimeout = "EXTERNAL_GATEWAY_TIMEOUT";
}