namespace backend.Shared.Results;

public class ApplicationResult
{
    public bool Success { get; init; }

    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public object? Meta { get; init; }

    public Dictionary<string, string[]> Errors { get; init; } = [];

    public static ApplicationResult Ok(string message, object? meta = null) =>
        new()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = message,
            Meta = meta
        };

    public static ApplicationResult BadRequest(string message, Dictionary<string, string[]> errors) =>
        new()
        {
            Success = false,
            StatusCode = StatusCodes.Status400BadRequest,
            Message = message,
            Errors = errors
        };

    public static ApplicationResult Unauthorized(string message, Dictionary<string, string[]> errors) =>
        new()
        {
            Success = false,
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = message,
            Errors = errors
        };

    public static ApplicationResult NotFound(string message, Dictionary<string, string[]> errors) =>
        new()
        {
            Success = false,
            StatusCode = StatusCodes.Status404NotFound,
            Message = message,
            Errors = errors
        };
}

public sealed class ApplicationResult<T> : ApplicationResult
{
    public T? Data { get; init; }

    public static ApplicationResult<T> Ok(T data, string message, object? meta = null) =>
        new()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = message,
            Data = data,
            Meta = meta
        };

    public new static ApplicationResult<T> BadRequest(string message, Dictionary<string, string[]> errors) =>
        new()
        {
            Success = false,
            StatusCode = StatusCodes.Status400BadRequest,
            Message = message,
            Errors = errors
        };

    public new static ApplicationResult<T> Unauthorized(string message, Dictionary<string, string[]> errors) =>
        new()
        {
            Success = false,
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = message,
            Errors = errors
        };

    public new static ApplicationResult<T> NotFound(string message, Dictionary<string, string[]> errors) =>
        new()
        {
            Success = false,
            StatusCode = StatusCodes.Status404NotFound,
            Message = message,
            Errors = errors
        };
}
