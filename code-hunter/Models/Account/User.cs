using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace code_hunter.Models.Account
{
    public class User : IdentityUser
    {
        [Key] public override string Email { get; set; }
        public bool Removed { get; set; }
    }
}