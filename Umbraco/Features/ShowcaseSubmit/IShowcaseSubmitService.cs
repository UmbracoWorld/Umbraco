namespace Umbraco.Features.ShowcaseSubmit;

public interface IShowcaseSubmitService
{
    Task<string> CreateShowcase(ShowcaseSubmitDto showcaseSubmitDto);
}