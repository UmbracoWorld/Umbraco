using Service.Showcase.Application.Common.Entities;

namespace Service.Showcase.Application.Showcase.Entities;


public record Showcase : Entity
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }

    public Guid AuthorId { get; set; }
}
