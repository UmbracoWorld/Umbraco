using Umbraco.Cms.Core.Notifications;

namespace Umbraco.Notifications.NewMemberRegistered;

/// <summary>
/// A notification fired when a member is registered.
/// </summary>
public class NewMemberRegisteredNotification : INotification
{
    public int MemberId { get; set; }
    public string Username { get; set; }
}

