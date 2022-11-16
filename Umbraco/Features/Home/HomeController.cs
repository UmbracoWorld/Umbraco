using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Common.Models;
using Umbraco.Common.Services;

namespace Umbraco.Features.Home;

public class HomeController : RenderController
{
    private readonly IShowcaseService _showcaseService;
    private readonly IAuthorInfoService _authorInfoService;

    public HomeController(ILogger<HomeController> logger, ICompositeViewEngine compositeViewEngine,
        IUmbracoContextAccessor umbracoContextAccessor, IShowcaseService showcaseService,
        IAuthorInfoService authorInfoService) 
        : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        _showcaseService = showcaseService;
        _authorInfoService = authorInfoService;
    }

    public async Task<IActionResult> Home()
    {
        var showcases = await _showcaseService.GetAllShowcases(4);

        foreach (var showcasesItem in showcases.Items)
        {
            showcasesItem.AuthorSummary = await _authorInfoService.GetMemberSummary(showcasesItem.AuthorId);
        }

        var contentModel = new Home(CurrentPage)
        {
            FeaturedShowcases = showcases.Items
        };

        return CurrentTemplate(contentModel);
    }
}