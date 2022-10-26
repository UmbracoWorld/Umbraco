namespace Umbraco.Features.MyAccount;

public interface IMyAccountPageService
{
    Task<MyAccount> GetInitialViewModelAsync();
}