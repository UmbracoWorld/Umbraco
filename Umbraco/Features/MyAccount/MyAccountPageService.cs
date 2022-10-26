using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Common.Constants;
using Member = UmbracoWorld.PublishedModels.Member;

namespace Umbraco.Features.MyAccount;

public class MyAccountPageService : IMyAccountPageService
{
    private readonly IMemberManager _memberManager;

    public MyAccountPageService(IMemberManager memberManager)
    {
        _memberManager = memberManager;
    }

    public async Task<MyAccount> GetInitialViewModelAsync()
    {
        var memberIdentityUser = await _memberManager.GetCurrentMemberAsync();
        if (memberIdentityUser == null)
        {
            return new MyAccount();
        }

        var publishedContent = _memberManager.AsPublishedMember(memberIdentityUser);
        if (publishedContent is not Member currentMember)
            return new MyAccount();

        var changeEmail = new ChangeEmail
        {
            Email = memberIdentityUser.Email,
            Key = currentMember.Key
        };

        var profileSettings = new ProfileSettings(currentMember);

        var viewModel = new MyAccount
        {
            ProfileSettings = profileSettings,
            EmailSettings = changeEmail,
            ProfilePicture = currentMember.ProfileImage,
            IsApproved = memberIdentityUser.IsApproved,
            CreatedDate = memberIdentityUser.CreatedDateUtc.ToLocalTime(),
            LastLoginDate = memberIdentityUser.LastLoginDateUtc?.ToLocalTime(),
            LastPasswordChangedDate = memberIdentityUser.LastPasswordChangeDateUtc?.ToLocalTime(),
            Key = memberIdentityUser.Key,
        };
        return viewModel;
    }
}