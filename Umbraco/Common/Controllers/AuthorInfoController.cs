using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Common.Models;
using Umbraco.Common.Services;

namespace Umbraco.Common.Controllers;


public class AuthorInfoController : UmbracoApiController
{
    private readonly IAuthorInfoService _authorInfoService;

    public AuthorInfoController(IAuthorInfoService authorInfoService)
    {
        _authorInfoService = authorInfoService;
    }

    public async Task<AuthorSummary> GetAuthorSummary(Guid key)
    {
        return await _authorInfoService.GetMemberSummary(key);
    }
}