using System.ComponentModel.DataAnnotations;

namespace code_hunter.Models.Account
{
    public class UserDto
    {
        public string Id { get; set; }
        [Required] public string Username { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required] public string Role { get; set; }

        public bool Removed { get; set; }
    }
}