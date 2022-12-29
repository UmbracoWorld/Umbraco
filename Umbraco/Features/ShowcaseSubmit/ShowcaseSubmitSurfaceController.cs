using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Common.Models.Dtos;
using Umbraco.Common.Services;

namespace Umbraco.Features.ShowcaseSubmit;

public class ShowcaseSubmitSurfaceController : SurfaceController
{
private readonly IShowcaseSubmitService _submitShowcaseService;
private readonly IMemberManager _memberManager;
    public ShowcaseSubmitSurfaceController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider, IShowcaseSubmitService submitShowcaseService, IMemberManager memberManager)
        : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _submitShowcaseService = submitShowcaseService;
        _memberManager = memberManager;
    }

    public async Task<IActionResult> SubmitShowcase(ShowcaseSubmitDto submitFormDto)
    {
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }

        var currentMember =  await _memberManager.GetCurrentMemberAsync();
        if (currentMember != null) 
            submitFormDto.AuthorId = currentMember.Key;
        
        var showcase = await _submitShowcaseService.CreateShowcase(submitFormDto);
        
        return Redirect($"/showcase/{showcase}");
    }

    [ResponseCache(NoStore = true, Duration = 0)]
    public IActionResult NewItem()
    {
        return PartialView("/Views/Partials/_showcaseImageHighlight.cshtml", new ImageHighlightFormDto());
    }
}