using System.Linq;
using System.Threading.Tasks;
using code_hunter.Context;
using code_hunter.Models.Question;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using code_hunter.Models.Rating;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace code_hunter.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RatingController : ControllerBase
    {
        private readonly CodeHunterContext _context;

        public RatingController(CodeHunterContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("rating")]
        public async Task<IActionResult> Rating()
        {
            var questions = await _context.Questions.FromSqlRaw(@"select q.""Id"",
            ""Votes"",
            ""AnswersCount"",
            ""Solved"",
            ""Title"",
            ""Description"",
            ""Removed"",
            ""CreatedAt"",
            ""UpdatedAt"",
            count(u.""IsUseful"") as ""Useful"",
            q.""UserId"",
            ""Username"",
            ""NotUseful""
            from ""Questions"" q
                left join ""UsefulQuestions"" u on q.""Id"" = u.""QuestionId""
            where u.""IsUseful""
            group by q.""Id""
            ORDER BY ""Useful"" desc
                limit 100;").ToListAsync();

            var answers = await _context.Answers.FromSqlRaw(@"select a.""Id"",
            ""Title"",
            ""Description"",
            ""Removed"",
            ""CreatedAt"",
            ""UpdatedAt"",
            count(u.""IsUseful"") as ""Useful"",
            a.""UserId"",
            ""Username"",
            ""NotUseful"",
            ""QuestionId""
            from ""Answers"" a
                left join ""UsefulAnswers"" u on a.""Id"" = u.""AnswerId""
            where u.""IsUseful""
            group by a.""Id""
            ORDER BY ""Useful"" desc
                limit 100;").ToListAsync();

            var usefulQUsers = await _context.Users.FromSqlRaw(@"select u.""Id"",
               u.""Email"",
               u.""Roles"",
               u.""CreatedAt"",
               u.""UpdatedAt"",
               u.""Removed"",
               u.""UserName"",
               u.""NormalizedUserName"",
               u.""NormalizedEmail"",
               u.""EmailConfirmed"",
               u.""PasswordHash"",
               u.""SecurityStamp"",
               u.""ConcurrencyStamp"",
               u.""PhoneNumber"",
               u.""PhoneNumberConfirmed"",
               u.""TwoFactorEnabled"",
               u.""LockoutEnd"",
               u.""LockoutEnabled"",
               u.""AccessFailedCount"",
               coalesce(count(uq.""IsUseful""), 0) as ""UsefulQuestionsCount"",
               0 as ""UsefulAnswersCount""
            from ""AspNetUsers"" u
                     left join ""Questions"" q on u.""Id""::uuid = q.""UserId""
                     left join ""UsefulQuestions"" uq on uq.""QuestionId"" = q.""Id""
            where uq.""IsUseful""
            group by u.""Id""
            order by ""UsefulQuestionsCount"" desc
                limit 100;").ToListAsync();

            var usefulAUsers = await _context.Users.FromSqlRaw(@"select u.""Id"",
               u.""Email"",
               u.""Roles"",
               u.""CreatedAt"",
               u.""UpdatedAt"",
               u.""Removed"",
               u.""UserName"",
               u.""NormalizedUserName"",
               u.""NormalizedEmail"",
               u.""EmailConfirmed"",
               u.""PasswordHash"",
               u.""SecurityStamp"",
               u.""ConcurrencyStamp"",
               u.""PhoneNumber"",
               u.""PhoneNumberConfirmed"",
               u.""TwoFactorEnabled"",
               u.""LockoutEnd"",
               u.""LockoutEnabled"",
               u.""AccessFailedCount"",
               0 as ""UsefulQuestionsCount"",
               coalesce(count(ua.""IsUseful""), 0) as ""UsefulAnswersCount""
            from ""AspNetUsers"" u
                     left join ""Answers"" a on u.""Id""::uuid = a.""UserId""
                     left join ""UsefulAnswers"" ua on ua.""AnswerId"" = a.""Id""
            where ua.""IsUseful""
            group by u.""Id""
            order by ""UsefulAnswersCount"" desc
                limit 100;").ToListAsync();
            return Ok(new {questions, answers, usefulQUsers, usefulAUsers});
        }
    }
}