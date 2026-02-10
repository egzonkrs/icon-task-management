namespace Icon.Api.Contracts;

/// <summary>
/// Standard API response wrapper.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// The response data.
    /// </summary>
    public T? Data { get; init; }
    
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool IsSuccess { get; init; }
    
    /// <summary>
    /// Indicates whether the request failed.
    /// </summary>
    public bool IsFailed { get; init; }
    
    /// <summary>
    /// Dictionary of error codes and messages.
    /// </summary>
    public Dictionary<string, string>? Errors { get; init; }
    
    /// <summary>
    /// Dictionary of reason codes and messages.
    /// </summary>
    public Dictionary<string, string>? Reasons { get; init; }
}
