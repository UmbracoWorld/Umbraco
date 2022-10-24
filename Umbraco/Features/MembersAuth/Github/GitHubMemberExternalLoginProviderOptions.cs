﻿using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Common;
using Umbraco.Common.Models;
using Umbraco.Common.Services;

namespace Umbraco.Features.MembersAuth.Github;

// Highly recommend this article: https://poornimanayar.co.uk/blog/member-login-using-github-in-umbraco-9/
public class GitHubMemberExternalLoginProviderOptions : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
{
    public const string SchemeName = "GitHub";
    private readonly IToastNotificationService _toastNotificationService;


    public GitHubMemberExternalLoginProviderOptions(IToastNotificationService toastNotificationService)
    {
        _toastNotificationService = toastNotificationService;
    }

    public void Configure(string name, MemberExternalLoginProviderOptions options)
    {
        if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + SchemeName)
        {
            return;
        }

        Configure(options);
    }

    public void Configure(MemberExternalLoginProviderOptions options) =>
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
            OnAutoLinking = (autoLinkUser, loginInfo) =>
            {
                // You can customize the member before it's linked.
                // i.e. Modify the member's groups based on the Claims returned
                // in the externalLogin info
            },
            OnExternalLogin = (user, loginInfo) =>
            {
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