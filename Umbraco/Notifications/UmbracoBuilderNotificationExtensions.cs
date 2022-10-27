using Umbraco.Cms.Core.Notifications;
using Umbraco.Notifications.Handlers;
using Umbraco.Notifications.NewMemberRegistered;

namespace Umbraco.Notifications;

public static class UmbracoBuilderNotificationExtensions
{
    public static IUmbracoBuilder AddCustomNotifications(this IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<NewMemberRegisteredNotification, NewMemberRegisteredNotificationHandler>();
        builder.AddNotificationHandler<MemberSavingNotification, MemberSavingNotificationHandler>();
        return builder;
    }
}