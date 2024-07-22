namespace IdentityAPI.Models
{
    public class ResetPasswordModel
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
