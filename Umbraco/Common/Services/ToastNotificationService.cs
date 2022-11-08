using Umbraco.Common.Models;

namespace Umbraco.Common.Services;

public class ToastNotificationService : IToastNotificationService
{
    private readonly ITempDataService _tempDataService;

    public ToastNotificationService(ITempDataService tempDataService)
    {
        _tempDataService = tempDataService;
    }

    public void AddToast(ToastModel message)
    {
        _tempDataService.Add(ToastModel.Key, message);
    }
}