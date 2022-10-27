using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Features.MyAccount;

public interface IMyAccountPageService
{
    Task<MyAccount> GetInitialViewModelAsync(IPublishedContent? publishedContent);
}