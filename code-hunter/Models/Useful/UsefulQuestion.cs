using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace code_hunter.Models.Useful
{
    public class UsefulQuestion
    {
        [Key] public Guid Id { get; set; }
        [ForeignKey("AspNetUsers")] public Guid UserId { get; set; }
        [ForeignKey("Questions")] public Guid QuestionId { get; set; }
        public bool IsUseful { get; set; }
    }
}