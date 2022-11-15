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
    public HomeController(ILogger<HomeController> logger, ICompositeViewEngine compositeViewEngine,
        IUmbracoContextAccessor umbracoContextAccessor, IShowcaseService showcaseService) : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        _showcaseService = showcaseService;
    }

    public async Task<IActionResult> Home()
    {
        var showcases = await _showcaseService.GetAllShowcases(4);

        // TODO: Figure out how to do this more gracefully?
        
        foreach (var showcasesItem in showcases.Items)
        {
            showcasesItem.AuthorSummary = GetAuthorSummary(showcasesItem.AuthorId);
        }
        
        var contentModel = new Home(CurrentPage)
        {
            FeaturedShowcases = showcases.Items
        };

        return CurrentTemplate(contentModel);
    }

    //TODO: This absoloutely needs to be moved to some service
    private AuthorSummary GetAuthorSummary(Guid showcasesItemAuthorId)
    {
        var authorSummary = new AuthorSummary("https://api.lorem.space/image/face?w=150&h=150", "James Bond");

        return authorSummary;
    }
}