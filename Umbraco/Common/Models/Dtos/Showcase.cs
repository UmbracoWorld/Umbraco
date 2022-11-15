namespace Umbraco.Common.Models.Dtos;

public record Showcase : Entity
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    
    public string PublicUrl { get; set; }
    
    public int MajorVersion { get; set; }
    
    public int MinorVersion { get; set; }
    
    public int PatchVersion { get; set; }
    
    public IEnumerable<string> Features { get; set; }
    
    public ICollection<string> Sectors { get; set; }
    
    public ICollection<string> Hostings { get; set; }
    
    public Guid AuthorId { get; set; }
    
    public string ImageSource { get; set; }

    public AuthorSummary AuthorSummary { get; set; }
    
    public IEnumerable<ImageHighlight> ImageHighlights { get; set; }
}