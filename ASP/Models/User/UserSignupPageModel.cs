namespace ASP.Models.User
{
    public class UserSignupPageModel
    {
        public UserSignupFormModel? FormModel { get; set; }
        public Dictionary<string,string>? FormErrors { get; set; }
    }
}
