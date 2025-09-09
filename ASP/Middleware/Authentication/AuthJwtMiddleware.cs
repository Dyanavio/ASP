using ASP.Data;
using ASP.Services.Jwt;
using ASP.Services.Time;
using System.Security.Claims;
using System.Text.Json;

namespace ASP.Middleware.Authentication
{
    public class AuthJwtMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthJwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, 
            IJwtService jwtService, ILogger<AuthJwtMiddleware> logger)
        {
            if (context
                .Request
                .Headers
                .Authorization
                .FirstOrDefault(header => header?.StartsWith("Bearer ") ?? false) is string authHeader)
            {
                string jwt = authHeader[7..];
                try
                {
                    var (header, payload) = jwtService.DecodeJwt(jwt);
                    var payloadElement = (JsonElement)payload;
                    context.User = new ClaimsPrincipal(
                             new ClaimsIdentity(
                                 new Claim[]
                                 {
                                     new(ClaimTypes.Name, payloadElement.GetProperty("Name").GetString()!),
                                     new(ClaimTypes.Email, payloadElement.GetProperty("Email").GetString()!),
                                 },
                                 nameof(AuthJwtMiddleware)
                             )
                        );
                }
                catch(Exception e)
                {
                    logger.LogError("JWT decode error: " + e.Message);
                }
            }
            await _next(context);
        }
    }

    public static class AuthJwtMiddlewareExtension
    {
        public static IApplicationBuilder UseAuthJwt(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthJwtMiddleware>();
        }
    }
}
