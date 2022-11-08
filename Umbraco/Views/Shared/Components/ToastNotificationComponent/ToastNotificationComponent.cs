using Microsoft.AspNetCore.Mvc;
using Umbraco.Common;
using Umbraco.Common.Constants;
using Umbraco.Common.Models;
using Umbraco.Common.Services;

namespace Umbraco.Views.Shared.Components.ToastNotificationComponent;

public class ToastNotificationComponent : ViewComponent
{
    private readonly ITempDataService _tempDataService;

    public ToastNotificationComponent(ITempDataService tempDataService)
    {
        _tempDataService = tempDataService;
    }

    public Task<IViewComponentResult> InvokeAsync()
    {
        var toastModel = _tempDataService.Get<ToastModel>(TempDataConstants.ToastKey);
        return Task.FromResult<IViewComponentResult>(View(toastModel));
    }
}