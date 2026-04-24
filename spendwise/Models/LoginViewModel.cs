using System.ComponentModel.DataAnnotations;

namespace spendwise.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}
