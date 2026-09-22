namespace EmployeeManagement.Api.Idempotency;

/// <summary>
/// A completed response captured against an idempotency key, kept so that a retry can be replayed
/// verbatim.
/// </summary>
/// <param name="StatusCode">Status code the original request was answered with.</param>
/// <param name="ContentType">Content type of the captured body, when the response carried one.</param>
/// <param name="Location">Value of the <c>Location</c> header, when the response carried one.</param>
/// <param name="RequestHash">
/// SHA-256 of the request body that produced the response. A retry whose body hashes to a different
/// value is a different request that happens to reuse a key.
/// </param>
/// <param name="Body">Serialized response body of the original request.</param>
public sealed record IdempotencyEntry(
    int StatusCode,
    string? ContentType,
    string? Location,
    string RequestHash,
    byte[] Body);
