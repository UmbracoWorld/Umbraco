using Umbraco.Features.MyAccount;

namespace Umbraco.Features;

public static class DependencyInjection
{
    /// <summary>
    /// Extension method to register our services to UmbracoBuilder
    /// </summary>
    public static IUmbracoBuilder AddFeatureServices(this IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IMyAccountPageService, MyAccountPageService>();


        return builder;
    }
}