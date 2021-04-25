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
        public async Task<IActionResult> Get()
        {
            var users = new List<UserDto>();
            await _userManager.Users.ForEachAsync(user =>
                users.Add(new UserDto
                {
                    Id = user.Id, Email = user.Email, Removed = user.Removed, Username = user.UserName
                }));

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

            user.Email = userModel.Email;
            user.UserName = userModel.Username;
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