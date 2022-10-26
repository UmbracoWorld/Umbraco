using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Features.MyAccount;
using Umbraco.Features.Profile;
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
                        .ForUmbracoPage(FindContent);
                    endpoints.MapControllerRoute(
                            "Profile Page Controller",
                            "/u/{slug?}",
                            new { Controller = "UserProfilePage", Action = "Index" })
                        .ForUmbracoPage(FindContent);
                })
            });
        });
        return builder;
    }

    private static IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
    {
        var umbracoContextAccessor = actionExecutingContext.HttpContext.RequestServices
            .GetRequiredService<IUmbracoContextAccessor>();
        var publishedValueFallback = actionExecutingContext.HttpContext.RequestServices
            .GetRequiredService<IPublishedValueFallback>();

        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
        var productRoot = umbracoContext.Content.GetAtRoot().First().FirstChild<UserProfilePage>();

        return productRoot;
    }
}