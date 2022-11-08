using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Common;
using Umbraco.Common.Models;
using Umbraco.Common.Services;
using Umbraco.Notifications.NewMemberRegistered;
using UmbracoWorld.PublishedModels;

namespace Umbraco.Features.MembersAuth.Github;

// Highly recommend this article: https://poornimanayar.co.uk/blog/member-login-using-github-in-umbraco-9/
public class GitHubMemberExternalLoginProviderOptions : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
{
    public const string SchemeName = "GitHub";
    private readonly IToastNotificationService _toastNotificationService;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMediaUploadService _mediaUploadService;
    private readonly IMemberService _memberService;


    public GitHubMemberExternalLoginProviderOptions(IToastNotificationService toastNotificationService,
        IEventAggregator eventAggregator, IMediaUploadService mediaUploadService, IMemberService memberService)
    {
        _toastNotificationService = toastNotificationService;
        _eventAggregator = eventAggregator;
        _mediaUploadService = mediaUploadService;
        _memberService = memberService;
    }

    public void Configure(string name, MemberExternalLoginProviderOptions options)
    {
        if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + SchemeName)
        {
            return;
        }

        Configure(options);
    }

    public async void Configure(MemberExternalLoginProviderOptions options) =>
        options.AutoLinkOptions = new MemberExternalSignInAutoLinkOptions(
            // Must be true for auto-linking to be enabled
            autoLinkExternalAccount: true,

            // Optionally specify the default culture to create
            // the user as. If null it will use the default
            // culture defined in the web.config, or it can
            // be dynamically assigned in the OnAutoLinking
            // callback.
            defaultCulture: null,

            // Optionally specify the default "IsApprove" status. Must be true for auto-linking.
            defaultIsApproved: true,

            // Optionally specify the member type alias. Default is "Member"
            defaultMemberTypeAlias: Constants.Conventions.MemberTypes.DefaultAlias,

            // Optionally specify the member groups names to add the auto-linking user to.
            defaultMemberGroups: Array.Empty<string>()
        )
        {
            // Optional callback
            OnAutoLinking = (autoLinkUser, loginInfo) => { },
            OnExternalLogin = (user, loginInfo) =>
            {
                // We have to check if the user has 0 logins, and we know this is a new member.
                // The OnAutoLinking callback is called before the user is persisted
                // We need the user to be persisted before manipulating them
                if (user.Logins.Count == 0)
                {
                    var notification = new NewMemberRegisteredNotification
                    {
                        Username = user.UserName,
                        ProviderName = loginInfo.ProviderDisplayName,
                        DisplayName = user.Name!
                    };
                    _eventAggregator.Publish(notification);
                }

                var toast = new ToastModel
                {
                    Message = $"Successfully logged in with {loginInfo.ProviderDisplayName}",
                    Title = "Logged in"
                };

                _toastNotificationService.AddToast(toast);


                return true; //returns a boolean indicating if sign in should continue or not.
            }
        };


}