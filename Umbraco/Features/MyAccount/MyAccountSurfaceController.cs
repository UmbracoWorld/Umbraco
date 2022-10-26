using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Member = UmbracoWorld.PublishedModels.Member;

namespace Umbraco.Features.MyAccount;

public class MyAccountSurfaceController : SurfaceController
{
    private readonly IMemberManager _memberManager;
    private readonly IMemberService _memberService;
    private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

    public MyAccountSurfaceController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IMemberManager memberManager, IMemberService memberService, IPublishedSnapshotAccessor publishedSnapshotAccessor)
        : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _memberManager = memberManager;
        _memberService = memberService;
        _publishedSnapshotAccessor = publishedSnapshotAccessor;
    }

    public string GetMemberPropertyAlias(Expression<Func<Member, object>> expression)
    {
        // throw an exception because it should never error
        return Member.GetModelPropertyType(_publishedSnapshotAccessor, expression)?.Alias ?? throw new InvalidOperationException();
    }

    private void AddErrors(IdentityResult result, string key)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(key, error.Description);
        }
    }

    public async Task<IActionResult> HandleUpdateProfileSettings(ProfileSettings profileSettings)
    {
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }
        
        var memberIdentityUser = await _memberManager.GetUserAsync(HttpContext.User);
        var currentMember = _memberService.GetByKey(memberIdentityUser.Key);
        if (currentMember == null)
        {
            return RedirectToCurrentUmbracoPage();
        }
        currentMember.SetValue(GetMemberPropertyAlias(member => member.GithubSocialLink!), profileSettings.SocialGithub);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.TwitterSocialLink!), profileSettings.SocialTwitter);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.ShowEmailOnProfile!), profileSettings.ShowEmailOnProfile);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.FavouriteDrink!), profileSettings.FavouriteDrink);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.FavouriteFood!), profileSettings.FavouriteFood);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.JobTitle!), profileSettings.JobTitle);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.City!), profileSettings.City);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.Country!), profileSettings.Country);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.CompanyName!), profileSettings.CompanyName);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.Bio!), profileSettings.AboutMe);
        currentMember.SetValue(GetMemberPropertyAlias(member => member.PersonalSocialLink!), profileSettings.Website);
        
        _memberService.Save(currentMember);

        return RedirectToCurrentUmbracoPage();
    }

    public async Task<IActionResult> HandleUpdateAccountSettings(ChangeEmail accountSettings)
    {
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }

        var currentMember = await _memberManager.GetUserAsync(HttpContext.User);
        if (currentMember == null!)
        {
            // this shouldn't happen, we also don't want to return an error so just redirect to where we came from
            return RedirectToCurrentUmbracoPage();
        }

        currentMember.Email = accountSettings.Email;
        currentMember.UserName = accountSettings.Email;

        var result = await _memberManager.UpdateAsync(currentMember);

        if (result.Succeeded)
        {
            return RedirectToCurrentUmbracoPage();
        }

        AddErrors(result, "HandleUpdateAccountSettings");
        return CurrentUmbracoPage();
    }
}