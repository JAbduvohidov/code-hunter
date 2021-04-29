using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace code_hunter.Models.Answer
{
    public class Answer : Body
    {
        [ForeignKey("Questions")] public Guid QuestionId { get; set; }
    }
}