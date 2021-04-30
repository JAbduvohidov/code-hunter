using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using code_hunter.Context;
using code_hunter.Models;
using code_hunter.Models.Account;
using code_hunter.Models.Answer;
using code_hunter.Models.Question;
using code_hunter.Models.Vote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace code_hunter.Controllers
{
    [Route("api/questions/{questionId}/vote")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly CodeHunterContext _context;
        private readonly UserManager<User> _userManager;

        public VotesController(CodeHunterContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Vote([FromRoute] Guid questionId)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(questionId) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var vote = await _context.Votes
                .Where(v => v.UserId.Equals(new Guid(userId)) && v.QuestionId.Equals(questionId))
                .FirstOrDefaultAsync();

            if (vote != null)
            {
                _context.Votes.Remove(vote);
                await _context.SaveChangesAsync();
                return Ok();
            }

            vote = new Vote
            {
                UserId = new Guid(userId),
                QuestionId = questionId,
            };

            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}