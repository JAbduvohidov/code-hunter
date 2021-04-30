using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace code_hunter.Models.Vote
{
    public class Vote
    {
        [Key] public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("Questions")] public Guid QuestionId { get; set; }
    }
}