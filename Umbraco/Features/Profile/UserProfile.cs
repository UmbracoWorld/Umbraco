using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Member = UmbracoWorld.PublishedModels.Member;

namespace Umbraco.Features.Profile;

public class UserProfile : ContentModel
{
    public UserProfile(IPublishedContent? content, Member member) : base(content)
    {
        Member = member;
    }
    public Member Member { get; set; }
}