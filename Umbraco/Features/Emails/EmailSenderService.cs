using Microsoft.Extensions.Options;
using MimeKit;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;

namespace Umbraco.Features.Emails;

/// <summary>
///  A class to encapsulate all emails sent from application
/// </summary>
public class EmailSenderService : IEmailSenderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailSender _emailSender;
    private readonly GlobalSettings _globalSettings;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private static class TemplateKeys
    {
        public const string Heading = "@heading";
        public const string ButtonHref = "@buttonHref";
        public const string ButtonText = "@buttonText";
        public const string Body = "@bodyParagraph";
        public const string DomainName = "@domainName";
    }

    public EmailSenderService(IEmailSender emailSender, IOptions<GlobalSettings> globalSettings,
        IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
        _globalSettings = globalSettings.Value;
    }
    
    private static string GetAbsoluteUrl(HttpRequest request)
    {
        return $"{request.Scheme}://{request.Host}";
    }
    
    /// <summary>
    /// We do some string replacing to get our dynamic content into the template.
    /// The template is a simple transactional email that has a heading, body and button.
    ///
    /// @heading = top heading
    /// @buttonHref = button href
    /// @buttonText = the text of the button
    /// @bodyParagraph - the body text
    /// @domainName = used to reference images in wwwroot
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetBaseEmailHtmlTemplate()
    {
        var wwwRoot = _webHostEnvironment.WebRootPath;
        var templatePath = $"{wwwRoot}\\templates\\index.html";
        var domainName = GetAbsoluteUrl(_httpContextAccessor.HttpContext?.Request);

        var builder = new BodyBuilder();

        using (var sourceReader = File.OpenText(templatePath))
        {
            builder.HtmlBody = await sourceReader.ReadToEndAsync();
        }

        var transformedString = builder.HtmlBody.Replace(TemplateKeys.DomainName, domainName);

        return transformedString;
    }

    public async Task SendConfirmEmail(EmailTemplate template)
    {
        const string subject = "Confirm your email address";

        var emailHtmlTemplate = await GetBaseEmailHtmlTemplate();

        emailHtmlTemplate = emailHtmlTemplate.Replace(TemplateKeys.Body,
            $"Confirm your email address by clicking the below button");
        
        emailHtmlTemplate = emailHtmlTemplate.Replace(TemplateKeys.ButtonHref, template.ButtonHref);
        emailHtmlTemplate = emailHtmlTemplate.Replace(TemplateKeys.ButtonText, template.ButtonText);
        emailHtmlTemplate = emailHtmlTemplate.Replace(TemplateKeys.Heading, subject);

        var message = new EmailMessage(_globalSettings.Smtp?.From, template.To, subject, emailHtmlTemplate, true);
        await _emailSender.SendAsync(message, "Confirm Email", false);
    }
}