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


        [HttpPost]
        [Route("answers/{answerId}/useful")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ToggleAnswerUseful([FromRoute] Guid answerId)
        {
            var answer = await _context.Answers.Where(a => a.Id.Equals(answerId) && a.Removed == false)
                .FirstOrDefaultAsync();
            if (answer == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"answer not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var useful = await _context.UsefulAnswers
                .Where(u => u.UserId.Equals(new Guid(userId)) && u.AnswerId.Equals(answerId))
                .FirstOrDefaultAsync();

            if (useful != null)
            {
                if (!useful.IsUseful)
                {
                    useful.IsUseful = true;
                    _context.UsefulAnswers.Update(useful);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

                _context.UsefulAnswers.Remove(useful);
                await _context.SaveChangesAsync();
                return Ok();
            }

            useful = new UsefulAnswer
            {
                UserId = new Guid(userId),
                AnswerId = answerId,
                IsUseful = true
            };

            await _context.UsefulAnswers.AddAsync(useful);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("answers/{answerId}/unuseful")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ToggleAnswerUnuseful([FromRoute] Guid answerId)
        {
            var answer = await _context.Answers.Where(a => a.Id.Equals(answerId) && a.Removed == false)
                .FirstOrDefaultAsync();
            if (answer == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"answer not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var useful = await _context.UsefulAnswers
                .Where(a => a.UserId.Equals(new Guid(userId)) && a.AnswerId.Equals(answerId))
                .FirstOrDefaultAsync();

            if (useful != null)
            {
                if (useful.IsUseful)
                {
                    useful.IsUseful = false;
                    _context.UsefulAnswers.Update(useful);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

                _context.UsefulAnswers.Remove(useful);
                await _context.SaveChangesAsync();
                return Ok();
            }

            useful = new UsefulAnswer
            {
                UserId = new Guid(userId),
                AnswerId = answerId,
                IsUseful = false
            };

            await _context.UsefulAnswers.AddAsync(useful);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}