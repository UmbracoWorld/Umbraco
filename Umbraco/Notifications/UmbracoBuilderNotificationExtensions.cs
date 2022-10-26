using Umbraco.Notifications.NewMemberRegistered;

namespace Umbraco.Notifications;

public static class UmbracoBuilderNotificationExtensions
{
    public static IUmbracoBuilder AddCustomNotifications(this IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<NewMemberRegisteredNotification, NewMemberRegisteredNotificationHandler>();

        return builder;
    }
}