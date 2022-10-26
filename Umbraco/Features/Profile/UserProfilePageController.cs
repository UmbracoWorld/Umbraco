using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Member = UmbracoWorld.PublishedModels.Member;

namespace Umbraco.Features.Profile;

public class UserProfile : ContentModel
{
    public UserProfile(IPublishedContent? content) : base(content)
    {
    }
    public Member Member { get; set; }
}

public class UserProfilePageController : UmbracoPageController
{
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IPublishedValueFallback _publishedValueFallback;
    private readonly IMemberManager _memberManager;
    private readonly IMemberService _memberService;
    private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
    public UserProfilePageController(ILogger<UserProfilePageController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IPublishedValueFallback publishedValueFallback, IMemberService memberService, IPublishedSnapshotAccessor publishedSnapshotAccessor, IMemberManager memberManager) : base(logger, compositeViewEngine)
    {
        _umbracoContextAccessor = umbracoContextAccessor;
        _publishedValueFallback = publishedValueFallback;
        _memberService = memberService;
        _publishedSnapshotAccessor = publishedSnapshotAccessor;
        _memberManager = memberManager;
    }
    
    public async Task<IActionResult> Index(string slug)
    {
        Func<IMember, bool> Predicate(string slugAlias)
        {
            return x => x.GetValue<string>(slugAlias)!.Equals(slug, StringComparison.OrdinalIgnoreCase);
        }

        // Probably want to not use member service here, maybe examine?
        var allMembers = _memberService.GetAllMembers();

        var slugAlias = Member.GetModelPropertyType(_publishedSnapshotAccessor, m => m.Slug)?.Alias!;
        var matchingMember = allMembers.FirstOrDefault(Predicate(slugAlias));
        if (matchingMember is null)
            return View("/Views/UserProfilePage.cshtml");


        if (_memberManager.AsPublishedMember(await _memberManager.FindByEmailAsync(matchingMember.Email)) is not Member member) 
            return View("/Views/UserProfilePage.cshtml");
        
        var userProfile = new UserProfile(CurrentPage)
        {
            Member = member
        };
        
        return View("/Views/UserProfilePage.cshtml", userProfile);

    }
}