using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace Umbraco.Features.MyAccount;

public class MyAccountPageController : RenderController
{
    private readonly IMyAccountPageService _myAccountPageService;
    public MyAccountPageController(ILogger<MyAccountPageController> logger, 
                                   ICompositeViewEngine compositeViewEngine,
                                   IUmbracoContextAccessor umbracoContextAccessor, 
                                   IMyAccountPageService myAccountPageService) 
        : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        _myAccountPageService = myAccountPageService;
    }
    
    public async Task<IActionResult> MyAccountPage()
    {
        var viewModel = await _myAccountPageService.GetInitialViewModelAsync();
        
        return CurrentTemplate(viewModel);
    }
}