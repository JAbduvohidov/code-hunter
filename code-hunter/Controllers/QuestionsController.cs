using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using code_hunter.Context;
using code_hunter.Models;
using code_hunter.Models.Account;
using code_hunter.Models.Question;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace code_hunter.Controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly CodeHunterContext _context;
        private readonly UserManager<User> _userManager;

        public QuestionsController(CodeHunterContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var questions = await _context.Questions.Where(q => q.Removed == false).ToListAsync();
            return Ok(questions);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(id) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var answers = await _context.Answers.Where(a => a.QuestionId.Equals(question.Id) && a.Removed == false)
                .ToListAsync();
            question.Answers = answers;
            return Ok(question);
        }

        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Add([FromBody] QuestionDto questionModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;
            var username = (await _userManager.Users.Where(u => u.Id.Equals(userId)).FirstAsync()).UserName;

            var question = new Question
            {
                Title = questionModel.Title,
                Description = questionModel.Description,
                Removed = false,
                Solved = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Votes = 0,
                Useful = 0,
                NotUseful = 0,
                AnswersCount = 0,
                UserId = new Guid(userId),
                Username = username
            };

            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] QuestionDto questionModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var question = await _context.Questions.Where(q => q.Id.Equals(id) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var role = (await _userManager.GetRolesAsync(new User {Id = userId})).First();
            if (!question.UserId.Equals(new Guid(userId)) || !role.Equals("Admin"))
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"can't edit this question"}});

            question.Title = questionModel.Title;
            question.Description = questionModel.Description;
            question.UpdatedAt = DateTime.Now;

            _context.Questions.Update(question);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(id) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var role = (await _userManager.GetRolesAsync(new User {Id = userId})).First();
            if (!question.UserId.Equals(new Guid(userId)) || !role.Equals("Admin"))
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"can't delete this question"}});

            question.Removed = true;
            question.UpdatedAt = DateTime.Now;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPut]
        [Route("{id}/solved")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ToggleSolved([FromRoute] Guid id)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(id) && q.Removed == false)
                .FirstOrDefaultAsync();
            if (question == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"question not found"}});

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            var role = (await _userManager.GetRolesAsync(new User {Id = userId})).First();
            if (!question.UserId.Equals(new Guid(userId)) || !role.Equals("Admin"))
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"can't delete this question"}});

            question.Solved = !question.Solved;
            question.UpdatedAt = DateTime.Now;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}