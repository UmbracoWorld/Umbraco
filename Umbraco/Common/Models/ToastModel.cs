using Umbraco.Common.Constants;

namespace Umbraco.Common.Models;

public class ToastModel
{
    public static string Key => TempDataConstants.ToastKey;
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; }
}