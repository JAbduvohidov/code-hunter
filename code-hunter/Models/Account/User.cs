using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace code_hunter.Models.Account
{
    public sealed class User : IdentityUser
    {
        [Key] public override string Email { get; set; }

        public List<string> Roles { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool Removed { get; set; }

        public int UsefulQuestionsCount { get; set; }

        public int UsefulAnswersCount { get; set; }
    }
}