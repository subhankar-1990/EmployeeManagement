namespace EmployeeManagement.Domain.Common;

/// <summary>
/// A single page of a larger result set together with the metadata needed to navigate it.
/// </summary>
/// <typeparam name="T">Type of the items contained in the page.</typeparam>
/// <remarks>
/// Lives in the domain because it is the vocabulary shared by the persistence abstractions
/// (declared by the application layer) and their implementations (infrastructure layer).
/// </remarks>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Creates a page of results.
    /// </summary>
    /// <param name="items">Items contained in this page.</param>
    /// <param name="pageNumber">One based page number.</param>
    /// <param name="pageSize">Maximum number of items per page.</param>
    /// <param name="totalCount">Total number of matching items across every page.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="items"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// When <paramref name="pageNumber"/> or <paramref name="pageSize"/> is less than one, or when
    /// <paramref name="totalCount"/> is negative.
    /// </exception>
    public PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);

        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>Items contained in this page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>One based page number.</summary>
    public int PageNumber { get; }

    /// <summary>Maximum number of items per page.</summary>
    public int PageSize { get; }

    /// <summary>Total number of matching items across every page.</summary>
    public int TotalCount { get; }

    /// <summary>Number of pages required to hold <see cref="TotalCount"/> items.</summary>
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

    /// <summary>Indicates that a page exists before this one.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Indicates that a page exists after this one.</summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>Creates an empty page, used when a filter matches nothing.</summary>
    /// <param name="pageNumber">One based page number.</param>
    /// <param name="pageSize">Maximum number of items per page.</param>
    public static PagedResult<T> Empty(int pageNumber, int pageSize) => new([], pageNumber, pageSize, 0);
}
