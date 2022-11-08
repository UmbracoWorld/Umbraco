using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Web.Common.Models;

namespace Umbraco.Features.MembersAuth;

public class LoginModel : PostRedirectModel
{
    [Required]
    [Display(Name = "Email address")]
    public string Username { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [StringLength(256)]
    public string Password { get; set; } = null!;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}