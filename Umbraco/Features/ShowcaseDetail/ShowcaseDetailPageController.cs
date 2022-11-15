using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Common.Models;
using Umbraco.Common.Models.Dtos;
using Umbraco.Common.Services;

namespace Umbraco.Features.ShowcaseDetail;

// We use the UmbracoPageController here so that we can add our own endpoints. 
// But we also include the IRenderController marker interface so Umbraco knows if we hit the proper route
// it can handle it normally. 
public class ShowcaseDetailPageController : UmbracoPageController, IRenderController
{
    private readonly IShowcaseService _showcaseService;

    public ShowcaseDetailPageController(ILogger<ShowcaseDetailPageController> logger,
        ICompositeViewEngine compositeViewEngine,
        IShowcaseService showcaseService) : base(logger, compositeViewEngine)
    {
        _showcaseService = showcaseService;
    }

    public async Task<IActionResult> Index(string id)
    {
        var showcase = await _showcaseService.GetShowcaseById(id);

        if (showcase is not null)
        {
            showcase.AuthorSummary = new AuthorSummary("https://api.lorem.space/image/face?w=150&h=150", "James Bond");
        }

        var contentModel = new ShowcaseDetail(CurrentPage)
        {
            Showcase = showcase
        };


        return View("/Views/ShowcaseDetailPage.cshtml", contentModel);
    }
}