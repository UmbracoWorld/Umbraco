namespace Umbraco.Common.Models;

public class AuthorSummary
{
    public AuthorSummary(string name)
    {
        Name = name;
    }

    public string? ProfilePictureSource { get; set; }
    public string ProfilePictureAlt => Name + " profile picture";
    public string Name { get; }
    public string Slug { get; set; }
}