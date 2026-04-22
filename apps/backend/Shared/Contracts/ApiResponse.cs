namespace backend.Shared.Contracts;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default);

public sealed record ApiErrorResponse(
    bool Success,
    string Message,
    IDictionary<string, string[]> Errors);
