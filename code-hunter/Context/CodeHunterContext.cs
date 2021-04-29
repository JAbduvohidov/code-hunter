using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using code_hunter.Models.Account;
using code_hunter.Models.Answer;
using code_hunter.Models.Question;

namespace code_hunter.Context
{
    public class CodeHunterContext : IdentityDbContext<User>
    {
        public CodeHunterContext(DbContextOptions<CodeHunterContext> options) : base(options)
        {
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
    }
}