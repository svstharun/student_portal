using System.ComponentModel.DataAnnotations;

namespace StudentPortalDBFirst.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Old password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(3, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
