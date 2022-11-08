using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Models;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace Umbraco.Features.MembersAuth;

public class RegistrationModel : PostRedirectModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    /// <summary>
    ///     The member type alias to use to register the member
    /// </summary>
    [Editable(false)]
    public string MemberTypeAlias => Constants.Conventions.MemberTypes.DefaultAlias;

    /// <summary>
    ///     The members desired display name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The members password
    /// </summary>
    [Required]
    [StringLength(256)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}