namespace Service.Showcase.Tests;

public class BaseTest
{
    public static class ShowcaseTestConstants
    {
        public static string Title => "this is a title";
        public static string Summary => "this is a summary";
        public static string Description => "this is a description";
    }

    protected static Application.Showcase.Entities.Showcase GetNewShowcase()
    {
        return new Application.Showcase.Entities.Showcase
        {
            Id = Guid.Empty,
            Title = ShowcaseTestConstants.Title,
            Description = ShowcaseTestConstants.Description,
            Summary = ShowcaseTestConstants.Summary,
            AuthorId = Guid.Empty,
        };
    }
}
