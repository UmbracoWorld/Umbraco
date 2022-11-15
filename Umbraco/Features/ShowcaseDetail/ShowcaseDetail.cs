using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Common.Models.Dtos;

namespace Umbraco.Features.ShowcaseDetail;

public class ShowcaseDetail : ContentModel
{
    public ShowcaseDetail(IPublishedContent? content) : base(content)
    {
    }

    public Showcase? Showcase { get; set; }
}