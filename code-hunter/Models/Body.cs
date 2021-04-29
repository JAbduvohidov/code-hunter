using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace code_hunter.Models
{
    public class Body
    {
        [Key] public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Removed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Useful { get; set; }
        public int NotUseful { get; set; }
        [ForeignKey("AspNetUsers")] public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}