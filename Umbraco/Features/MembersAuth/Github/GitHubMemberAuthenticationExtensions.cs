namespace Umbraco.Features.MembersAuth.Github;

public static class GitHubMemberAuthenticationExtensions
{
    public static IUmbracoBuilder AddGitHubMemberAuthentication(this IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<GitHubMemberExternalLoginProviderOptions>();
        builder.AddMemberExternalLogins(logins =>
        {
            logins.AddMemberLogin(
                memberAuthenticationBuilder =>
                {
                    memberAuthenticationBuilder.AddGitHub(

                        memberAuthenticationBuilder.SchemeForMembers(GitHubMemberExternalLoginProviderOptions.SchemeName),
                        options =>
                        {
                            options.ClientId = builder.Config.GetValue<string>("ExternalProviderCredentials:GithubClientID");
                            options.ClientSecret = builder.Config.GetValue<string>("ExternalProviderCredentials:GithubClientSecret");
                            
                            // although the "user" scope says it returns the email, it's in the wrong claim format
                            // so we must specify the "user:email" claim to allow auto-linking.
                            options.Scope.Add("user");
                            options.Scope.Add("user:email");
                        });
                });
        });
        return builder;
    }
}