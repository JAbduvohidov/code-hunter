using System;

namespace code_hunter.Models.Account
{
    public class AuthResultDto : ErrorsModel<string>
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public Guid UserId { get; set; }
        public bool Success { get; set; }
    }
}