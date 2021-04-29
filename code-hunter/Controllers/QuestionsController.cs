using System;
using System.Linq;
using System.Threading.Tasks;
using code_hunter.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace code_hunter.Controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly CodeHunterContext _context;

        public QuestionsController(CodeHunterContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var questions = await _context.Questions.Select(q => q).ToListAsync();
            return Ok(questions);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var question = await _context.Questions.Where(q => q.Id.Equals(id)).FirstOrDefaultAsync();
            var answers = await _context.Answers.Where(a => a.QuestionId.Equals(question.Id)).ToListAsync();
            question.Answers = answers;
            return Ok();
        }

        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Add()
        {
            //TODO: create QuestionDto model with validation properties
            //TODO: get Question details from [FromBody] argument
            //TODO: use this userId to add new question to database
            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;

            //TODO: build Question model and fill its data with info from QuestionDto
            //TODO: save new Question to database

            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Edit()
        {
            //TODO: get QuestionDto from [FromBody] argument
            //TODO: get userId from HttpContext.User.Claims
            //TODO: fill updated question fields
            //TODO: update UpdatedAt field with DateTime.Now
            //TODO: save updated info
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete()
        {
            //TODO: get questionId from [FromQuery] argument
            //TODO: get question from database
            //TODO: updated Removed field to false
            //TODO: updated UpdatedAt field to DateTime.Now
            //TODO: save updated info to database
            return Ok();
        }
    }
}