using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using UmbracoWorld.PublishedModels;

namespace Umbraco.Common.Services;

public interface IMediaUploadService
{
    string CreateMediaItemFromFileStream(Stream stream, string fileName);
}

public class MediaUploadService : IMediaUploadService
{
    private readonly IMediaService _mediaService;
    private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
    private readonly MediaFileManager _mediaFileManager;
    private readonly MediaUrlGeneratorCollection _mediaUrlGeneratorCollection;
    private readonly IShortStringHelper _shortStringHelper;

    public MediaUploadService(IMediaService mediaService,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider, MediaFileManager mediaFileManager,
        MediaUrlGeneratorCollection mediaUrlGeneratorCollection, IShortStringHelper shortStringHelper)
    {
        _mediaService = mediaService;
        _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        _mediaFileManager = mediaFileManager;
        _mediaUrlGeneratorCollection = mediaUrlGeneratorCollection;
        _shortStringHelper = shortStringHelper;
    }

    public string CreateMediaItemFromFileStream(Stream stream, string fileName)
    {
        var mediaItem = _mediaService.CreateMedia(fileName, -1, Image.ModelTypeAlias);
        mediaItem.SetValue(_mediaFileManager, _mediaUrlGeneratorCollection, _shortStringHelper,
            _contentTypeBaseServiceProvider, Cms.Core.Constants.Conventions.Media.File, fileName,
            stream);
        
        _mediaService.Save(mediaItem);

        return mediaItem.GetUdi().ToString();
    }
    
}