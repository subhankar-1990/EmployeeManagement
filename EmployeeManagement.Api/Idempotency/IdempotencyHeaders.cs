namespace EmployeeManagement.Api.Idempotency;

/// <summary>
/// The headers that make up the idempotency contract of a write endpoint.
/// </summary>
public static class IdempotencyHeaders
{
    /// <summary>
    /// Request header that carries the client generated key, named after the IETF
    /// <c>Idempotency-Key</c> header field draft.
    /// </summary>
    public const string Key = "Idempotency-Key";

    /// <summary>
    /// Response header set to <c>true</c> when the response was replayed from a stored entry instead
    /// of the write running a second time.
    /// </summary>
    public const string Replayed = "Idempotency-Replayed";
}
