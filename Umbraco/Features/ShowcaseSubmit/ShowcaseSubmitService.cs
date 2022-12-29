using Umbraco.Common.Models.Dtos;
using Umbraco.Common.Services;

namespace Umbraco.Features.ShowcaseSubmit;

public class ShowcaseSubmitService : IShowcaseSubmitService
{
    private readonly IShowcaseService _showcaseService;
    private readonly IMediaUploadService _mediaUploadService;

    public ShowcaseSubmitService(IShowcaseService showcaseService, IMediaUploadService mediaUploadService)
    {
        _showcaseService = showcaseService;
        _mediaUploadService = mediaUploadService;
    }

    public async Task<string> CreateShowcase(ShowcaseSubmitDto showcaseSubmitDto)
    {
        // upload image source to media
        var imageSourceMedia = _mediaUploadService.CreateShowcaseImageSourceMedia(showcaseSubmitDto.ImageSource,
            showcaseSubmitDto.Title, showcaseSubmitDto.AuthorId.ToString());

        // create showcase
        var showcase = new Showcase
        {
            Title = showcaseSubmitDto.Title,
            Summary = showcaseSubmitDto.Summary,
            Description = showcaseSubmitDto.Description,
            PublicUrl = showcaseSubmitDto.PublicUrl,
            MajorVersion = showcaseSubmitDto.MajorVersion,
            MinorVersion = showcaseSubmitDto.MinorVersion,
            PatchVersion = showcaseSubmitDto.PatchVersion,
            Features = showcaseSubmitDto.Features ?? new List<string>(),
            Sectors = showcaseSubmitDto.Sectors ?? new List<string>(),
            Hostings = showcaseSubmitDto.Hostings ?? new List<string>(),
            ImageSource = imageSourceMedia,
            ImageHighlights = GetImageHighlights(showcaseSubmitDto.ImageHighlights, showcaseSubmitDto.AuthorId),
            AuthorId = showcaseSubmitDto.AuthorId,
        };

        // send to endpoint and get id
        var showcaseId = await _showcaseService.CreateShowcase(showcase);

        return showcaseId.Id;
    }

    private IEnumerable<ImageHighlight> GetImageHighlights(List<ImageHighlightFormDto>? imageHighlights, Guid authorId)
    {
        var imageHighlightList = new List<ImageHighlight>();
        if (imageHighlights == null) 
            return imageHighlightList;
        
        foreach (var imageHighlight in imageHighlights)
        {
            var imageHighlightMedia =
                _mediaUploadService.CreateShowcaseImageHighlightMedia(imageHighlight.ImageSource,
                    authorId.ToString());
            imageHighlightList.Add(new ImageHighlight
            {
                Title = imageHighlight.Title,
                Description = imageHighlight.Description,
                Source = imageHighlightMedia
            });
        }

        return imageHighlightList;
    }
}