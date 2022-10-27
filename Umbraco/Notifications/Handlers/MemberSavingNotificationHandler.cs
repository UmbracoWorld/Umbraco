using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Common.Extensions;
using UmbracoWorld.PublishedModels;

namespace Umbraco.Notifications.Handlers;

public class MemberSavingNotificationHandler : INotificationHandler<MemberSavingNotification>
{
    private readonly IMemberService _memberService;
    private readonly string _slugAlias;

    public MemberSavingNotificationHandler(IMemberService memberService, IPublishedSnapshotAccessor publishedSnapshotAccessor)
    {
        _memberService = memberService;
        _slugAlias = Member.GetModelPropertyType(publishedSnapshotAccessor, m => m.Slug)?.Alias!;
    }

    public void Handle(MemberSavingNotification notification)
    {
        foreach (var member in notification.SavedEntities)
        {
            // generate a nice slug for the member
            var memberSlug = member.Name!.ToSlugFriendly();
            
            // find any existing members with the same slug
            var membersWithSameSlug = 
                _memberService.GetMembersByPropertyValue(_slugAlias, memberSlug)
                    ?.ToList();
            
            // append some numbers for uniqueness
            if (membersWithSameSlug != null && membersWithSameSlug.Any())
            {
                // we add one here so you don't get Slug0 
                memberSlug = memberSlug + (membersWithSameSlug.Count + 1);
            }
            
            member.SetValue(_slugAlias, memberSlug);
        }
    }
}