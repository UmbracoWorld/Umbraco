using Umbraco.Common.Models;

namespace Umbraco.Common.Services;

public interface IAuthorInfoService
{
    Task<AuthorSummary> GetMemberSummary(Guid key);
}