namespace IdentityAPI.Models
{
    public class RegisterModel
    {
        required public string Username { get; set; }
        required public string Password { get; set; }
        required public string EmailAddress { get; set; }
    }
}
