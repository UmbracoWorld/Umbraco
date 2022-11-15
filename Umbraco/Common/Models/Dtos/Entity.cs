namespace Umbraco.Common.Models.Dtos;

public record Entity
{
    public string Id { get; init; }
    public DateTime DateCreated { get; init; }
    public DateTime DateModified { get; init; }
}