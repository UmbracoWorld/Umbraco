namespace Umbraco.Common.Models;

public class AuthorSummary
{
    public AuthorSummary(string profilePictureSource, string name)
    {
        ProfilePictureSource = profilePictureSource;
        Name = name;
    }

    public string ProfilePictureSource { get; }
    public string ProfilePictureAlt => Name + " profile picture";
    public string Name { get; }
}