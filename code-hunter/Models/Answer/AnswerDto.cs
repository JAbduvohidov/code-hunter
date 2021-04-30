using System;
using System.ComponentModel.DataAnnotations;

namespace code_hunter.Models.Answer
{
    public class AnswerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        [Required, MinLength(3)] public string Description { get; set; }
        public bool Removed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}