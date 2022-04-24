using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using code_hunter.Context;
using code_hunter.Models;
using code_hunter.Models.Account;
using code_hunter.Models.Answer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace code_hunter.Controllers
{
    [Route("api/questions/{questionId:guid}/answers")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly CodeHunterContext _context;
        private readonly UserManager<User> _userManager;

        public AnswersController(CodeHunterContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Add([FromRoute] Guid questionId, [FromBody] AnswerDto answerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var question = await _context.Questions.Where(q => q.Id.Equals(questionId) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;
            var username = await _userManager.Users.Where(u => u.Id.Equals(userId)).Select(u => u.UserName)
                .FirstOrDefaultAsync();

            var answer = new Answer
            {
                Title = answerModel.Title,
                Description = answerModel.Description,
                UserId = new Guid(userId),
                Username = username,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                QuestionId = questionId,
                Removed = false,
                Useful = 0,
                NotUseful = 0
            };

            await _context.Answers.AddAsync(answer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Edit([FromRoute] Guid questionId, [FromRoute] Guid id,
            [FromBody] AnswerDto answerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var question = await _context.Questions.Where(q => q.Id.Equals(questionId) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var answer = await _context.Answers
                .Where(a => a.Id.Equals(id) && a.QuestionId.Equals(questionId) && a.Removed == false)
                .FirstOrDefaultAsync();
            if (answer == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"answer not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var role = (await _userManager.GetRolesAsync(new User {Id = userId})).First();
            if (!question.UserId.Equals(new Guid(userId)) || !role.Equals("Admin"))
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"can't edit this answer"}});

            answer.Title = answerModel.Title;
            answer.Description = answerModel.Description;
            answer.UpdatedAt = DateTime.Now;

            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete([FromRoute] Guid questionId, [FromRoute] Guid id)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(questionId) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var answer = await _context.Answers
                .Where(a => a.Id.Equals(id) && a.QuestionId.Equals(questionId) && a.Removed == false)
                .FirstOrDefaultAsync();
            if (answer == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"answer not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var role = (await _userManager.GetRolesAsync(new User {Id = userId})).First();
            if (!question.UserId.Equals(new Guid(userId)) || !role.Equals("Admin"))
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"can't delete this question"}});

            answer.Removed = true;
            answer.UpdatedAt = DateTime.Now;
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}