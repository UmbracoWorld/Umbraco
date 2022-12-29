using System.ComponentModel.DataAnnotations;

namespace Umbraco.Features.ShowcaseSubmit;

public class ShowcaseSubmitDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Summary { get; set; }
    [Required]
    public string Description { get; set; }

    public string PublicUrl { get; set; }

    [Required]
    public int MajorVersion { get; set; }
    [Required]
    public int MinorVersion { get; set; }
    [Required]
    public int PatchVersion { get; set; }

    public IEnumerable<string>? Features { get; set; }

    public ICollection<string>? Sectors { get; set; }

    public ICollection<string>? Hostings { get; set; }

    public Guid AuthorId { get; set; }

    public IFormFile ImageSource { get; set; }

    public List<ImageHighlightFormDto>? ImageHighlights { get; set; } = new List<ImageHighlightFormDto>();
}