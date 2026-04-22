namespace backend.Shared.Results;

public class ApplicationResult
{
    public bool Success { get; init; }

    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public Dictionary<string, string[]> Errors { get; init; } = [];

    public static ApplicationResult Ok(string message) =>
        new()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = message
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

}

public sealed class ApplicationResult<T> : ApplicationResult
{
    public T? Data { get; init; }

    public static ApplicationResult<T> Ok(T data, string message) =>
        new()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = message,
            Data = data
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

}
