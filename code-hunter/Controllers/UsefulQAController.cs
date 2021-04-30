using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using code_hunter.Context;
using code_hunter.Models;
using code_hunter.Models.Useful;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace code_hunter.Controllers
{
    [Route("api/questions/{questionId}")]
    [ApiController]
    public class UsefulQAController : ControllerBase
    {
        private readonly CodeHunterContext _context;

        public UsefulQAController(CodeHunterContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("useful")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ToggleQuestionUseful([FromRoute] Guid questionId)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(questionId) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var useful = await _context.UsefulQuestions
                .Where(u => u.UserId.Equals(new Guid(userId)) && u.QuestionId.Equals(questionId))
                .FirstOrDefaultAsync();

            if (useful != null)
            {
                if (!useful.IsUseful)
                {
                    useful.IsUseful = true;
                    _context.UsefulQuestions.Update(useful);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

                _context.UsefulQuestions.Remove(useful);
                await _context.SaveChangesAsync();
                return Ok();
            }

            useful = new UsefulQuestion
            {
                UserId = new Guid(userId),
                QuestionId = questionId,
                IsUseful = true
            };

            await _context.UsefulQuestions.AddAsync(useful);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("unuseful")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ToggleQuestionUnuseful([FromRoute] Guid questionId)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(questionId) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var useful = await _context.UsefulQuestions
                .Where(u => u.UserId.Equals(new Guid(userId)) && u.QuestionId.Equals(questionId))
                .FirstOrDefaultAsync();

            if (useful != null)
            {
                if (useful.IsUseful)
                {
                    useful.IsUseful = false;
                    _context.UsefulQuestions.Update(useful);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

                _context.UsefulQuestions.Remove(useful);
                await _context.SaveChangesAsync();
                return Ok();
            }

            useful = new UsefulQuestion
            {
                UserId = new Guid(userId),
                QuestionId = questionId,
                IsUseful = false
            };

            await _context.UsefulQuestions.AddAsync(useful);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}