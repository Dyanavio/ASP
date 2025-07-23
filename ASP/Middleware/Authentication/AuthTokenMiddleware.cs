using ASP.Data;
using ASP.Data.Entities;
using ASP.Middleware.Authentication;
using ASP.Services.Time;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace ASP.Middleware.Authentication
{
    public class AuthTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITimeService _timeService;
        public AuthTokenMiddleware(RequestDelegate next, ITimeService timeService)
        {
            _next = next;
            _timeService = timeService;
        }
        public async Task InvokeAsync(HttpContext context, DataContext dataContext)
        {
            if (context
                .Request
                .Headers
                .Authorization
                .FirstOrDefault(header => header?.StartsWith("Bearer ") ?? false) is string authHeader)
            {
                string jti = authHeader[7..];

                if (dataContext.AccessTokens
                              .AsNoTracking()
                              .Include(at => at.UserAccess)
                              .ThenInclude(ua => ua.UserData).OrderByDescending(at => at.Iat)
                              .FirstOrDefault(at => at.Jti == jti) is AccessToken accessToken)
                {
                    if ((long)Convert.ToDouble(accessToken.Exp) < ((DateTime.Now.Ticks - DateTime.UnixEpoch.Ticks) / (long)1e7))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Session expired. Forced logout.");
                        Console.ResetColor();
                    }
                    else
                    {
                        context.User = new ClaimsPrincipal(
                             new ClaimsIdentity(
                                 new Claim[]
                                 {
                                 new(ClaimTypes.Name, accessToken.UserAccess.UserData.Name),
                                 new(ClaimTypes.Email, accessToken.UserAccess.UserData.Email),
                                 },
                                 nameof(AuthTokenMiddleware)
                             )
                        );
                    }
                        
                }
                
            }
            await _next(context);
        }
    }

    public static class AuthTokenMiddlewareExtension
    {
        public static IApplicationBuilder UseAuthToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthTokenMiddleware>();
        }
    }
}


