using Umbraco.Cms.Core.Notifications;

namespace Umbraco.Notifications.NewMemberRegistered;

/// <summary>
/// A notification fired when a member is registered.
/// </summary>
public class NewMemberRegisteredNotification : INotification
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string ProviderName { get; set; }
}

