using System.ComponentModel.DataAnnotations;

namespace code_hunter.Models.Account
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "passwords doesn't match")]
        public string ConfirmPassword { get; set; }
    }
}