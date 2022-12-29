using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Common.Services;
using Umbraco.Features.Emails;
using Umbraco.Features.MyAccount;
using Umbraco.Features.Profile;
using Umbraco.Features.ShowcaseSubmit;
using UmbracoWorld.PublishedModels;

namespace Umbraco.Features;

public static class DependencyInjection
{
    /// <summary>
    /// Extension method to register our services to UmbracoBuilder
    /// </summary>
    public static IUmbracoBuilder AddFeatureServices(this IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IMyAccountPageService, MyAccountPageService>();
        builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
        builder.Services.AddSingleton<IShowcaseSubmitService, ShowcaseSubmitService>();

        builder.Services.Configure<UmbracoPipelineOptions>(options =>
        {
            options.AddFilter(new UmbracoPipelineFilter(nameof(UserProfilePageController))
            {
                Endpoints = app => app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                            "Profile Page Controller",
                            "/user/{slug?}",
                            new { Controller = "UserProfilePage", Action = "Index" })
                        .ForUmbracoPage(context => FindContent(UserProfilePage.ModelTypeAlias, context));
                    endpoints.MapControllerRoute(
                            "Profile Page Controller",
                            "/u/{slug?}",
                            new { Controller = "UserProfilePage", Action = "Index" })
                        .ForUmbracoPage(context => FindContent(UserProfilePage.ModelTypeAlias, context));
                    endpoints.MapControllerRoute(
                            "Showcase Detail Controller",
                            "/s/{id?}",
                            new { Controller = "ShowcaseDetailPage", Action = "Index" })
                        .ForUmbracoPage(context => FindContent(ShowcaseDetailPage.ModelTypeAlias, context));
                    endpoints.MapControllerRoute(
                            "Showcase Detail Controller",
                            "/showcase/{id?}",
                            new { Controller = "ShowcaseDetailPage", Action = "Index" })
                        .ForUmbracoPage(context => FindContent(ShowcaseDetailPage.ModelTypeAlias, context));
                })
            });
        });
        return builder;
    }

    private static IPublishedContent FindContent(string contentTypeAlias, ActionExecutingContext actionExecutingContext)
    {
        var umbracoContextAccessor = actionExecutingContext.HttpContext.RequestServices.GetRequiredService<IUmbracoContextAccessor>();

        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
        var firstChildOfType = umbracoContext.Content?.GetAtRoot().DescendantsOrSelfOfType(contentTypeAlias).FirstOrDefault();

        return firstChildOfType;
    }
    
    // private static IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
    // {
    //     var umbracoContextAccessor = actionExecutingContext.HttpContext.RequestServices
    //         .GetRequiredService<IUmbracoContextAccessor>();
    //
    //     var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
    //     var productRoot = umbracoContext.Content.GetAtRoot().First().FirstChild<UserProfilePage>();
    //
    //     return productRoot;
    // }
}