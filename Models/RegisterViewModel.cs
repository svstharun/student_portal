using System.ComponentModel.DataAnnotations;

namespace StudentPortalDBFirst.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please provide the first name")]
        [Display(Name = "First Name baa")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; } = null!; // null-forgiving operator

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Mobile Number baa")]
        [Phone]
        public string? Mobile { get; set; }

        [Required]
        //[Display(Name = "Password baa")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        //[Display(Name = "Confirm baa")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }

   

}

