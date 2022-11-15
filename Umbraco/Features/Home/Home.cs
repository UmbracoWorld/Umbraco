using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Common.Models.Dtos;

namespace Umbraco.Features.Home;

public class Home : ContentModel
{
    public Home(IPublishedContent? content) : base(content)
    {
    }

    public IEnumerable<Showcase>? FeaturedShowcases { get; init; }
}