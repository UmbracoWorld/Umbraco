using System.ComponentModel.DataAnnotations;

namespace Umbraco.Features.ShowcaseSubmit;

public class ImageHighlightFormDto
{
    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    [Required] public IFormFile ImageSource { get; set; }
}