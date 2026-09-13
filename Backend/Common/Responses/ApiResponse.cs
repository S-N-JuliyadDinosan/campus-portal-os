namespace CampusServicesPortal.Common.Responses;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    IReadOnlyCollection<string>? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new(true, message, data);

    public static ApiResponse<T> Fail(
        string message,
        IReadOnlyCollection<string>? errors = null) =>
        new(false, message, default, errors);
}

public static class ApiResponse
{
    public static ApiResponse<object> Ok(string message = "Success") =>
        ApiResponse<object>.Ok(new { }, message);

    public static ApiResponse<object> Fail(
        string message,
        IReadOnlyCollection<string>? errors = null) =>
        ApiResponse<object>.Fail(message, errors);
}
