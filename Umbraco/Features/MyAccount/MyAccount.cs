using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Member = UmbracoWorld.PublishedModels.Member;

namespace Umbraco.Features.MyAccount;

public class MyAccount : ContentModel
{
    public ChangeEmail EmailSettings { get; set; }
    public ProfileSettings ProfileSettings { get; set; }
    public MediaWithCrops? ProfilePicture { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastPasswordChangedDate { get; set; }
    public Guid Key { get; set; }

    public MyAccount(IPublishedContent content) : base(content)
    {
    }
}

public class ChangeEmail
{
    public Guid Key { get; set; }

    [Required] [EmailAddress] public string Email { get; set; }

    [Required]
    [EmailAddress]
    [Compare(nameof(Email))]
    public string ConfirmEmail { get; set; }
}

public class ProfileSettings
{
    public ProfileSettings()
    {
    }

    public ProfileSettings(Member currentMember)
    {
        AboutMe = currentMember.Bio;
        City = currentMember.City;
        Country = currentMember.Country;
        CompanyName = currentMember.CompanyName;
        JobTitle = currentMember.JobTitle;
        DisplayName = currentMember.Name;
        FavouriteDrink = currentMember.FavouriteDrink;
        FavouriteFood = currentMember.FavouriteFood;
        SocialGithub = currentMember.GithubSocialLink;
        SocialTwitter = currentMember.TwitterSocialLink;
        ShowEmailOnProfile = currentMember.ShowEmailOnProfile;
        Website = currentMember.PersonalSocialLink;
    }

    public string? Country { get; set; }
    public string? City { get; set; }
    [Display(Name = "Favourite Food")] public string? FavouriteFood { get; set; }
    [Display(Name = "Favourite drink")] public string? FavouriteDrink { get; set; }
    [Display(Name = "Github")] public string? SocialGithub { get; set; }

    [Display(Name = "Twitter")] public string? SocialTwitter { get; set; }

    [MaxLength(250)]
    [Display(Name = "Bio")]
    public string? AboutMe { get; set; }

    [Display(Name = "Job title")] public string? JobTitle { get; set; }
    [Display(Name = "Company name")] public string? CompanyName { get; set; }
    [Display(Name = "Display Name")] public string? DisplayName { get; set; }
    public bool ShowEmailOnProfile { get; set; }
    public string? Website { get; set; }
}