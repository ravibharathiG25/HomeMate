namespace HomeMate.Shared.Results;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IReadOnlyList<string>? Errors { get; set; }
    public int StatusCode { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
        StatusCode = 200
    };

    public static ApiResponse<T> Created(T data, string? message = "Resource created successfully.") => new()
    {
        Success = true,
        Data = data,
        Message = message,
        StatusCode = 201
    };

    public static ApiResponse<T> Fail(string message, int statusCode = 400, IReadOnlyList<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        StatusCode = statusCode,
        Errors = errors
    };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string? message = "Operation succeeded.") => new()
    {
        Success = true,
        Message = message,
        StatusCode = 200
    };

    public static new ApiResponse Fail(string message, int statusCode = 400, IReadOnlyList<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        StatusCode = statusCode,
        Errors = errors
    };
}
