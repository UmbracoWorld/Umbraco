namespace Umbraco.Features.Emails;

public record EmailTemplate(string To, string ButtonHref, string ButtonText, string BodyText)
{
    public string To { get; set; } = To;
    public string ButtonHref { get; set; } = ButtonHref;
    public string ButtonText { get; set; } = ButtonText;
    public string BodyText { get; set; } = BodyText;
}