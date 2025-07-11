using ASP.Data.Entities;
using ASP.Data;
using ASP.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ASP.Services.Random;
using ASP.Services.Kdf;
using System.Text.RegularExpressions;

namespace ASP.Controllers
{
    public class UserController(
        IRandomService randomService, 
        IKdfService kdfService,
        DataContext dataContext,
        ILogger<UserController> logger) : Controller
    {
        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly DataContext _dataContext = dataContext;
        private readonly Regex _passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$"); // With God's help...
        private readonly ILogger<UserController> _logger = logger;
        public ViewResult SignUp()
        {
            UserSignupPageModel pageModel = new();
            if(HttpContext.Session.Keys.Contains("UserSignupFormModel"))
            {
                pageModel.FormModel = JsonSerializer.Deserialize<UserSignupFormModel>(HttpContext.Session.GetString("UserSignupFormModel")!)!;
                pageModel.FormErrors = ProcessSignUpData(pageModel.FormModel);
                HttpContext.Session.Remove("UserSignupFormModel"); // Deleting so that the data is not processed again upon returning to the page
            }
            return View(pageModel);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Register(UserSignupFormModel model)
        {
            HttpContext.Session.SetString("UserSignupFormModel",
                JsonSerializer.Serialize(model)); // Saving
            return RedirectToAction(nameof(SignUp)); 
        }

        private Dictionary<string, string> ProcessSignUpData(UserSignupFormModel model)
        {
            Dictionary<string, string> errors = [];
            #region Validation
            if (string.IsNullOrEmpty(model.UserName))
            {
                errors[nameof(model.UserName)] = "Name cannot be empty";
            }
            if(string.IsNullOrEmpty(model.UserEmail))
            {
                errors[nameof(model.UserEmail)] = "Email cannot be empty";
            }
            if (string.IsNullOrEmpty(model.UserLogin))
            {
                errors[nameof(model.UserLogin)] = "Login cannot be empty";
            }
            else
            {
                if(model.UserLogin.Contains(':'))
                {
                    errors[nameof(model.UserLogin)] = "Login cannot contain ':'";
                }
                else
                {
                    if(_dataContext.UserAccesses.Any(ua => ua.Login == model.UserLogin))
                    {
                        errors[nameof(model.UserLogin)] = "Login is already in use";
                    }
                }
            }
            if(string.IsNullOrEmpty(model.UserPassword))
            {
                errors[nameof(model.UserPassword)] = "Password cannot be empty";
                errors[nameof(model.UserRepeat)] = "Invalid original password";
            }
            else
            {
                if(!_passwordRegex.IsMatch(model.UserPassword))
                {
                    errors[nameof(model.UserPassword)] = "Password must be at least 12 characters long and contain lower, upper case letters, at least one number and at least one special character";
                    errors[nameof(model.UserRepeat)] = "Invalid original password";
                }
                else
                {
                    if (model.UserRepeat != model.UserPassword)
                    {
                        errors[nameof(model.UserRepeat)] = "Passwords must match";
                    }
                }
            }
            if (!(model.Agree))
            {
                errors[nameof(model.Agree)] = "You must accept terms and conditions";
            }

            #endregion

            if (errors.Count == 0)
            {
                Guid userId = Guid.NewGuid();

                UserData user = new()
                {
                    Id = userId,
                    Name = model.UserName,
                    Email = model.UserEmail,
                    Birthdate = model.Birthdate,
                    RegisteredAt = DateTime.Now,
                };
                String salt = _randomService.Otp(12); // TODO: add salt generator
                UserAccess userAccess = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Login = model.UserLogin,
                    Salt = salt,
                    Dk = _kdfService.Dk(model.UserPassword, salt),
                    RoleId = "SelfRegistered"
                };
                // adding new object to context
                _dataContext.Database.BeginTransaction();
                _dataContext.Users.Add(user);
                _dataContext.UserAccesses.Add(userAccess);
                try
                {
                    _dataContext.SaveChanges();
                    _dataContext.Database.CommitTransaction();
                }
                catch(Exception e)
                {
                    _logger.LogError("ProcessSignupData: {e}", e.Message);
                    _dataContext.Database.RollbackTransaction();
                    errors["500"] = "Could not save. Try again later";
                }
            }
            return errors;
        }
    }
}
