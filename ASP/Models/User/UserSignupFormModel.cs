using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.User
{
    public class UserSignupFormModel
    {
        [FromForm(Name = "user-name")]
        public string UserName { get; set; } = null!;

        [FromForm(Name = "user-email")]
        public string UserEmail { get; set; } = null!;

        [FromForm(Name = "birthdate")]
        public DateTime? Birthdate { get; set; } = null!;

        [FromForm(Name = "user-login")]
        public string UserLogin { get; set; } = null!;

        [FromForm(Name = "user-password")]
        public string UserPassword { get; set; } = null!;

        [FromForm(Name = "user-repeat")]
        public string UserRepeat { get; set; } = null!;

        [FromForm(Name = "agree")]
        public bool Agree { get; set; } = false;
    }
}
