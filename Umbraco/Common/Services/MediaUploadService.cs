using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common;
using UmbracoWorld.PublishedModels;
using Folder = UmbracoWorld.PublishedModels.Folder;

namespace Umbraco.Common.Services;

public interface IMediaUploadService
{
    string CreateMediaItemFromFileStream(Stream stream, string fileName, int parentId = -1);
    int CreateMediaFolderIfNotExists(string folderName, int parentId = -1);
    string CreateShowcaseImageSourceMedia(IFormFile file, string name, string authorId);
    string CreateShowcaseImageHighlightMedia(IFormFile file, string authorId);
}

public class MediaUploadService : IMediaUploadService
{
    private readonly IMediaService _mediaService;
    private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
    private readonly MediaFileManager _mediaFileManager;
    private readonly MediaUrlGeneratorCollection _mediaUrlGeneratorCollection;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IUmbracoHelperAccessor _umbracoHelperAccessor;

    private const string ShowcaseImagesFolderName = "ShowcaseImages";

    public MediaUploadService(IMediaService mediaService,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider, MediaFileManager mediaFileManager,
        MediaUrlGeneratorCollection mediaUrlGeneratorCollection, IShortStringHelper shortStringHelper, IUmbracoHelperAccessor umbracoHelperAccessor)
    {
        _mediaService = mediaService;
        _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        _mediaFileManager = mediaFileManager;
        _mediaUrlGeneratorCollection = mediaUrlGeneratorCollection;
        _shortStringHelper = shortStringHelper;
        _umbracoHelperAccessor = umbracoHelperAccessor;
    }

    public string CreateMediaItemFromFileStream(Stream stream, string fileName, int parentId = -1)
    {
        var mediaItem = _mediaService.CreateMedia(fileName, parentId, Image.ModelTypeAlias);
        mediaItem.SetValue(_mediaFileManager, _mediaUrlGeneratorCollection, _shortStringHelper,
            _contentTypeBaseServiceProvider, Cms.Core.Constants.Conventions.Media.File, fileName,
            stream);

        _mediaService.Save(mediaItem);

        return mediaItem.GetUdi().ToString();
    }

    public string CreateShowcaseImageSourceMedia(IFormFile file, string name, string authorId)
    {
        // we can just manually control the folder structure from here.
        var showcaseImagesFolder = CreateMediaFolderIfNotExists(ShowcaseImagesFolderName, -1);
        var authorFolder = CreateMediaFolderIfNotExists(authorId, showcaseImagesFolder);

        // convert file to Stream
        var stream = file.OpenReadStream();
        //get file extension
        var extension = Path.GetExtension(file.FileName);
        var udi = CreateMediaItemFromFileStream(stream, name + extension, authorFolder);
        
        _umbracoHelperAccessor.TryGetUmbracoHelper(out var umbracoHelper);

        var url = umbracoHelper.Media(udi).MediaUrl();

        return url;
    }

    public string CreateShowcaseImageHighlightMedia(IFormFile file, string authorId)
    {
        // we can just manually control the folder structure from here.
        var showcaseImagesFolder = CreateMediaFolderIfNotExists(ShowcaseImagesFolderName, -1);
        var authorFolder = CreateMediaFolderIfNotExists(authorId, showcaseImagesFolder);
        var highlightFolder = CreateMediaFolderIfNotExists("Highlight", authorFolder);
        
        // convert file to Stream
        var stream = file.OpenReadStream();
        //get file extension
        var extension = Path.GetExtension(file.FileName);
        var udi = CreateMediaItemFromFileStream(stream, "Highlight" + extension, highlightFolder);
        
        _umbracoHelperAccessor.TryGetUmbracoHelper(out var umbracoHelper);
        
        var url = umbracoHelper.Media(udi).MediaUrl();
        
        return url;
    }

    public int CreateMediaFolderIfNotExists(string folderName, int parentId = -1)
    {
        var rootFolder = _mediaService
            .GetPagedDescendants(parentId, 0, int.MaxValue, out var totalRecords)
            .FirstOrDefault(x => x.Name == folderName);

        if (rootFolder != null) 
            return rootFolder.Id;
        
        var newFolder = _mediaService.CreateMediaWithIdentity(folderName, parentId, Folder.ModelTypeAlias);
        return newFolder.Id;


    }
}