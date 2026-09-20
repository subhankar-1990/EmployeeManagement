namespace EmployeeManagement.Application.Contracts.Common;

/// <summary>
/// Envelope returned by every paginated collection endpoint.
/// </summary>
/// <typeparam name="T">Type of the items contained in the page.</typeparam>
public sealed class PagedResponse<T>
{
    /// <summary>Items contained in this page.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>One based page number that produced this page.</summary>
    public int PageNumber { get; init; }

    /// <summary>Maximum number of items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Total number of matching items across every page.</summary>
    public int TotalCount { get; init; }

    /// <summary>Number of pages required to hold <see cref="TotalCount"/> items.</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Indicates that a page exists before this one.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Indicates that a page exists after this one.</summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
