namespace Service.Showcase.Infrastructure.Databases.Showcases.Models;

internal abstract record Entity
{
    public Guid Id { get; init; }
    public DateTime DateCreated { get; init; }
    public DateTime DateModified { get; set; }
}