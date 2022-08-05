namespace Service.Showcase.Infrastructure.Databases.Showcases.Models;

internal record Showcase : Entity
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }

    public Guid AuthorId { get; set; }
}
