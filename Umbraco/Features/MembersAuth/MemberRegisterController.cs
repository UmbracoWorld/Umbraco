using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

namespace Umbraco.Features.MembersAuth;

public class MemberRegisterController : SurfaceController
{
    private readonly IMemberManager _memberManager;
    private readonly IMemberSignInManager _memberSignInManager;
    private readonly ICoreScopeProvider _scopeProvider;

    public MemberRegisterController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IMemberManager memberManager,
        IMemberSignInManager memberSignInManager,
        ICoreScopeProvider scopeProvider)
        : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _memberManager = memberManager;
        _memberSignInManager = memberSignInManager;
        _scopeProvider = scopeProvider;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleRegisterMember([Bind(Prefix = "registerModel")] RegistrationModel model)
    {
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }

        IdentityResult result = await RegisterMemberAsync(model);
        if (result.Succeeded)
        {
            TempData["FormSuccess"] = true;

            // If there is a specified path to redirect to then use it.
            if (!model.RedirectUrl.IsNullOrWhiteSpace())
            {
                return Redirect(model.RedirectUrl!);
            }

            // Redirect to current page by default.
            return RedirectToCurrentUmbracoPage();
        }

        AddErrors(result);
        return CurrentUmbracoPage();
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (IdentityError? error in result.Errors)
        {
            ModelState.AddModelError("registerModel", error.Description);
        }
    }


    /// <summary>
    ///     Registers a new member.
    /// </summary>
    /// <param name="model">Register member model.</param>
    /// <param name="logMemberIn">Flag for whether to log the member in upon successful registration.</param>
    /// <returns>Result of registration operation.</returns>
    private async Task<IdentityResult> RegisterMemberAsync(RegistrationModel model)
    {
        using var scope = _scopeProvider.CreateCoreScope(autoComplete: true);
        
        var identityUser = MemberIdentityUser
            .CreateNew(model.Email, model.Email, model.MemberTypeAlias, true, model.Name);
        
        IdentityResult identityResult = await _memberManager.CreateAsync(identityUser, model.Password);
        if (identityResult.Succeeded)
        {
            await _memberSignInManager.SignInAsync(identityUser, false);
        }

        return identityResult;
    }
}