namespace Umbraco.Common.Models.Dtos;

public class PaginatedList<T>
{
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }

    public IEnumerable<T> Items { get; set; } = new List<T>();

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;
}