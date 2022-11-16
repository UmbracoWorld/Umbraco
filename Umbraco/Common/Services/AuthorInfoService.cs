using Umbraco.Cms.Core.Security;
using Umbraco.Common.Models;

namespace Umbraco.Common.Services;

/// <summary>
///  A service encapsulating methods for getting details about an Author.
/// </summary>
public class AuthorInfoService : IAuthorInfoService
{
    private readonly IMemberManager _memberManager;

    public AuthorInfoService(IMemberManager memberManager)
    {
        _memberManager = memberManager;
    }

    public async Task<AuthorSummary> GetMemberSummary(Guid key)
    {
        var memberIdentityUser = await _memberManager.FindByIdAsync(key.ToString());
        var publishedMember = _memberManager.AsPublishedMember(memberIdentityUser);
        
        if (publishedMember is not UmbracoWorld.PublishedModels.Member member)
        {
            return new AuthorSummary("Anonymous")
            {
                Slug = "/not-found",
            };
        }

        var dto = new AuthorSummary(member.Name!)
        {
            Slug = member.Slug!,
            ProfilePictureSource = member.ProfileImage?.Url()
        };

        return dto;
    }
}