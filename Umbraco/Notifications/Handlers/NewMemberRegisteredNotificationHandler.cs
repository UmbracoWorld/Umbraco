using AspNet.Security.OAuth.GitHub;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Common.Services;
using Umbraco.Features.MembersAuth.Github;
using Umbraco.Notifications.NewMemberRegistered;

namespace Umbraco.Notifications.Handlers;

public class NewMemberRegisteredNotificationHandler : INotificationHandler<NewMemberRegisteredNotification>
{
    private readonly IMemberService _memberService;
    private readonly IMediaUploadService _mediaUploadService;
    private const string DefaultMemberGroupAlias = "Member";

    public NewMemberRegisteredNotificationHandler(IMemberService memberService, IMediaUploadService mediaUploadService)
    {
        _memberService = memberService;
        _mediaUploadService = mediaUploadService;
    }

    public void Handle(NewMemberRegisteredNotification notification)
    {
        _memberService.AssignRole(notification.Username, DefaultMemberGroupAlias);
        if (notification.ProviderName == GitHubMemberExternalLoginProviderOptions.SchemeName)
        {
            GetProfilePicture(notification.Username);
        }
    }
    
    private void GetProfilePicture(string userName)
    {
        var member = _memberService.GetByEmail(userName);
        if (member == null) 
            return;
        
        var fileName = member.Name + ".png";
        
        using var client = new HttpClient();
        var storedStream = client.GetStreamAsync($"https://github.com/{fileName}").Result;
        
        using var memoryStream = new MemoryStream();
        storedStream.CopyTo(memoryStream);
        
        var mediaUdi = _mediaUploadService.CreateMediaItemFromFileStream(memoryStream, fileName);
        
        member.SetValue("profileImage", mediaUdi);
        _memberService.Save(member);
    }
}