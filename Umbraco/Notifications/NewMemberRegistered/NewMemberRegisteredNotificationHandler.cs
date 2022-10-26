using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Notifications.NewMemberRegistered;

public class NewMemberRegisteredNotificationHandler : INotificationHandler<NewMemberRegisteredNotification>
{
    private readonly IMemberService _memberService;
    private const string DefaultMemberGroupAlias = "Member";

    public NewMemberRegisteredNotificationHandler(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public void Handle(NewMemberRegisteredNotification notification)
    {
        _memberService.AssignRole(notification.Username, DefaultMemberGroupAlias);
    }
}