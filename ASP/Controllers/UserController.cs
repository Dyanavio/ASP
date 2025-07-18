using ASP.Data.Entities;
using ASP.Data;
using ASP.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ASP.Services.Random;
using ASP.Services.Kdf;
using System.Text.RegularExpressions;
using System.Buffers.Text;
using Microsoft.EntityFrameworkCore;
using ASP.Services.Time;

namespace ASP.Controllers
{
    public class UserController(
        ITimeService timeService,
        IRandomService randomService, 
        IKdfService kdfService,
        DataContext dataContext,
        ILogger<UserController> logger) : Controller
    {
        private readonly ITimeService _timeService = timeService;
        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly DataContext _dataContext = dataContext;
        private readonly Regex _passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$"); // With God's help...
        private readonly ILogger<UserController> _logger = logger;

        // ============= LOGIN ============= //

        private UserAccess Authenticate()
        {
            string authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                throw new Exception("Missing 'Authorization' header");
            }
            string authScheme = "Basic ";
            if (!authHeader.StartsWith(authScheme))
            {
                throw new Exception($"Authorization scheme error: '{authScheme}' only");
            }
            string credentials = authHeader[authScheme.Length..]; // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            string decoded;
            try
            {
                decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
            }
            catch (Exception e)
            {
                _logger.LogError("SignIn: {e}", e.Message);
                throw new Exception($"Authorization credentials decode error");
            }
            string[] parts = decoded.Split(':', 2);
            if (parts.Length != 2)
            {
                throw new Exception($"Authorization credentials decompose error");
            }
            string login = parts[0];
            string password = parts[1];
            var userAccess = _dataContext
                .UserAccesses
                .AsNoTracking() // for readonly query, no connection to contex will be created
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole)
                .FirstOrDefault(ua => ua.Login == login);

            if (userAccess == null)
            {
                throw new Exception($"Authorization credentials rejected: invalid login");
            }
            if (_kdfService.Dk(password, userAccess.Salt) != userAccess.Dk)
            {
                throw new Exception($"Authorization credentials rejected: invalid password");
            }
            return userAccess;
        }
        [HttpGet]
        public JsonResult LogIn()
        {
            UserAccess userAccess;
            try
            {
                userAccess = Authenticate();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Status = 401,
                    Data = e.Message
                });
            }
            // The method has to create a token and send it
            // Tokens are digital 'certificates' that contain information about users
            // Tokens are divided into:
            // JWT - that have information
            // Bearer - that only have indentifiers of tokens

            //Creating a new token
            AccessToken accessToken = new()
            {
                Jti = Guid.NewGuid().ToString(),
                Sub = userAccess.Id,
                Iat = _timeService.Timestamp().ToString(),
                Exp = (_timeService.Timestamp() + (long)1e4).ToString(),
                Iss = nameof(ASP),
                Aud = userAccess.RoleId
            };

            return Json(new
            {
                Status = 200,
                Data = accessToken
            });
        }

            [HttpGet]
        public JsonResult SignIn()
        {
            UserAccess userAccess;
            try
            {
                userAccess = Authenticate();
            }
            catch(Exception e)
            {
                return Json(new
                { 
                    Status = 401,
                    Data = e.Message
                });
            }
            HttpContext.Session.SetString("userAccess", JsonSerializer.Serialize(userAccess));
            return Json(new {
                Status = 200,
                Data = "Ok"
            });
        }

        // ============= REGISTRATION ============= //
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
