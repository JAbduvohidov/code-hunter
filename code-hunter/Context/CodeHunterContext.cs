using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using code_hunter.Models.Account;

namespace code_hunter.Context
{
    public class CodeHunterContext : IdentityDbContext<User>
    {
        public CodeHunterContext(DbContextOptions<CodeHunterContext> options) : base(options)
        {
        }
    }
}