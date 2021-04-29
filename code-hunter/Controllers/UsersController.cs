using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using code_hunter.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using code_hunter.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace code_hunter.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get([FromQuery] string email, [FromQuery] int limit, [FromQuery] int offset)
        {
            email = email == null ? string.Empty : email.Trim();

            var users = await _userManager.Users.Where(user => email.Equals(string.Empty) || user.Email.Contains(email))
                .OrderByDescending(user => user.CreatedAt).Skip(offset).Take(limit)
                .Select(user =>
                    new UserDto
                    {
                        Id = user.Id, Email = user.Email, Removed = user.Removed, Username = user.UserName,
                        CreatedAt = user.CreatedAt, UpdatedAt = user.UpdatedAt
                    }).ToListAsync();

            users.ForEach(user =>
                user.Role = _userManager.GetRolesAsync(new User {Id = user.Id}).Result.First());

            return Ok(users);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UserDto userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = _userManager.Users.FirstOrDefault(p => p.Id.Equals(id.ToString()));
            if (user == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"user not found"}});

            var result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            if (!result.Succeeded) return InternalServerError(result);

            result = await _userManager.AddToRoleAsync(user, userModel.Role);
            if (!result.Succeeded) return InternalServerError(result);

            user.UserName = userModel.Username;
            user.UpdatedAt = DateTime.Now;
            result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return InternalServerError(result);

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var user = _userManager.Users.FirstOrDefault(p => p.Id.Equals(id.ToString()));
            if (user == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"user not found"}});

            user.Removed = true;
            user.UpdatedAt = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return InternalServerError(result);

            return Ok();
        }

        [NonAction]
        private ObjectResult InternalServerError(IdentityResult result) =>
            StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorsModel<IdentityError> {Errors = result.Errors});
    }
}