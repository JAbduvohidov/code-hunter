namespace code_hunter.Models.Account
{
    public class AuthResultDto : ErrorsModel<string>
    {
        public string Token { get; set; }
        public bool Success { get; set; }
    }
}