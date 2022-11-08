using Umbraco.Common.Models;

namespace Umbraco.Common.Services;

public interface IToastNotificationService
{
    void AddToast(ToastModel message);
}