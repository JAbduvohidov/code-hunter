using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using code_hunter.Models.Account;

namespace code_hunter.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AccountController(UserManager<User> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(AuthErrorResult("invalid payload"));

            var user = await _userManager.FindByEmailAsync(userModel.Email);
            if (user != null)
                return BadRequest(AuthErrorResult($"email '{user.Email}' can not be used"));

            var newUser = new User {Email = userModel.Email, UserName = userModel.Username, Removed = false};
            var isCreated = await _userManager.CreateAsync(newUser, userModel.Password);
            if (!isCreated.Succeeded)
                return BadRequest(new AuthResultDto
                {
                    Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                    Success = false
                });

            var jwtToken = GenerateJwtToken(newUser);

            return Ok(new AuthResultDto
            {
                Success = true,
                Token = jwtToken
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(AuthErrorResult("invalid payload"));

            var user = await _userManager.FindByEmailAsync(userModel.Email);

            if (user == null)
                return BadRequest(AuthErrorResult("invalid login request"));

            if (user.Removed)
                return BadRequest(AuthErrorResult("invalid login request"));

            var isCorrect = await _userManager.CheckPasswordAsync(user, userModel.Password);
            if (!isCorrect)
                return BadRequest(AuthErrorResult("invalid login request"));

            var jwtToken = GenerateJwtToken(user);

            return Ok(new AuthResultDto
            {
                Success = true,
                Token = jwtToken
            });
        }

        private static AuthResultDto AuthErrorResult(params string[] errors) =>
            new()
            {
                Errors = errors.ToList(),
                Success = false
            };

        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new("id", user.Id),
                    new(JwtRegisteredClaimNames.Email, user.Email),
                    new(JwtRegisteredClaimNames.Sub, user.Email),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}