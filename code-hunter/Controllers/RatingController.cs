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
        public IActionResult Rating()
        {
            var questions = _context.Questions.FromSqlRaw(@"select q.""Id"",
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
                limit 100;").ToList();

            var answers = _context.Answers.FromSqlRaw(@"select a.""Id"",
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
                limit 100;").ToList();
            return Ok(new {questions, answers});
        }
    }
}