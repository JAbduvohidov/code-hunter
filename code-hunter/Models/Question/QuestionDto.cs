using System;
using System.ComponentModel.DataAnnotations;

namespace code_hunter.Models.Question
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        [Required, MinLength(3)] public string Title { get; set; }
        [Required, MinLength(3)] public string Description { get; set; }
        public bool Removed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}