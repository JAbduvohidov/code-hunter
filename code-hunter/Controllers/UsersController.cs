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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;
            var user = await _userManager.Users.Where(u => u.Removed == false).Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.UserName,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
            }).FirstOrDefaultAsync(u => u.Id.Equals(userId));

            if (user == null)
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"user not found"}});

            user.Role = (await _userManager.GetRolesAsync(new User {Id = user.Id})).First();

            return Ok(user);
        }

        [HttpGet]
        [Route("organizations")]
        public async Task<IActionResult> GetOrganizations([FromQuery] string email, [FromQuery] int limit,
            [FromQuery] int offset)
        {
            email = email == null ? string.Empty : email.Trim();

            var users = await _userManager.Users.Where(user =>
                    (email.Equals(string.Empty) || user.Email.Contains(email)) && user.Removed == false)
                .OrderByDescending(user => user.CreatedAt)
                .Select(user =>
                    new UserDto
                    {
                        Id = user.Id, Email = user.Email, Removed = user.Removed, Username = user.UserName,
                        CreatedAt = user.CreatedAt, UpdatedAt = user.UpdatedAt
                    }).ToListAsync();

            users.ForEach(user =>
                user.Role = _userManager.GetRolesAsync(new User {Id = user.Id}).Result.First());

            var organizations = users.Where(u => u.Role.Equals("Organization")).ToList();

            return Ok(new
                {organizations = organizations.Skip(offset).Take(limit).ToList(), count = organizations.Count});
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get([FromQuery] string email, [FromQuery] int limit, [FromQuery] int offset)
        {
            email = email == null ? string.Empty : email.Trim();

            var u = await _userManager.Users.Where(user =>
                    (email.Equals(string.Empty) || user.Email.Contains(email)) && user.Removed == false)
                .OrderByDescending(user => user.CreatedAt)
                .Select(user =>
                    new UserDto
                    {
                        Id = user.Id, Email = user.Email, Removed = user.Removed, Username = user.UserName,
                        CreatedAt = user.CreatedAt, UpdatedAt = user.UpdatedAt
                    }).ToListAsync();

            var count = await _userManager.Users.Where(user =>
                    (email.Equals(string.Empty) || user.Email.Contains(email)) && user.Removed == false)
                .OrderByDescending(user => user.CreatedAt).CountAsync();

            u.ForEach(user =>
                user.Role = _userManager.GetRolesAsync(new User {Id = user.Id}).Result.First());

            count -= u.Count(uu => uu.Role.Equals("Organization"));

            var users = u.Where(uu => !uu.Role.Equals("Organization")).Skip(offset).Take(limit);

            return Ok(new {users, count});
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UserDto userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;
            var role = (await _userManager.GetRolesAsync(new User {Id = userId})).First();
            if (!role.Equals("Admin") && !userId.Equals(id.ToString()))
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"can't edit this user"}});

            var user = _userManager.Users.FirstOrDefault(u => u.Id.Equals(id.ToString()) && u.Removed == false);
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
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userId = HttpContext.User.Claims.First(c => c.Type.Equals("uid")).Value;
            var role = (await _userManager.GetRolesAsync(new User {Id = userId})).First();
            if (!role.Equals("Admin") && userId.Equals(id.ToString()))
                return BadRequest(new ErrorsModel<string> {Errors = new List<string> {"can't edit this user"}});

            var user = _userManager.Users.FirstOrDefault(u => u.Id.Equals(id.ToString()) && u.Removed == false);
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