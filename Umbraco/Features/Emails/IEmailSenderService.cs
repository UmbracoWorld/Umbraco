namespace Umbraco.Features.Emails;

public interface IEmailSenderService
{
    Task SendConfirmEmail(EmailTemplate template);
}