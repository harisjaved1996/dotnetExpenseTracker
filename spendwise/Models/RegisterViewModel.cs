using System.ComponentModel.DataAnnotations;

namespace spendwise.Models;

public class RegisterViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Terms & Conditions.")]
    [Display(Name = "I agree to the Terms & Conditions")]
    public bool AgreeToTerms { get; set; }
}
