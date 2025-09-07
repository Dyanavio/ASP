using ASP.Data.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace ASP.Middleware.Authentication
{
    public class AuthSessionMiddleware
    {
        private readonly RequestDelegate _next; 
        public AuthSessionMiddleware(RequestDelegate next) 
        { 
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if(context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove("userAccess");
                context.Response.Redirect(context.Request.Path);
                return;
            }
            else if (context.Session.Keys.Contains("userAccess"))
            {
                var ua = JsonSerializer.Deserialize<UserAccess>(context.Session.GetString("userAccess")!)!;
                // context.Items["userAccess"] = ua - Do not do this;
                // Sending data as entities to HttpContext creates high cohesion that can lead
                // to problems after creating a migration (i.e. changing entities) or switching data provider

                // Solution - use another model (of the HttpContext level)
                // context.User
                context.User = new ClaimsPrincipal( // 1 user - several identities
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            // We do so as there can be multiple sources of data
                            new(ClaimTypes.Name, ua.UserData.Name), // Mapping: changing model (creating a dictionary) from ua.UserAccess to the set of Claim(s)
                            new(ClaimTypes.Email, ua.UserData.Email),
                            new(ClaimTypes.Sid, ua.Login), // Sid - secure id = login
                            new(ClaimTypes.PrimarySid, ua.UserData.Id.ToString())
                        },
                        nameof(AuthSessionMiddleware) // Here we indicate the provider of data
                    )
                );
            }
            await _next(context);
        }
    }
}


// === HTTP REQUEST ===
/*
 Browser                                                                                        Server
     HTTP ------------------------------------------------------------------------------------> [WebServer: IIS/Kestrel]
                                                                                                Forms HttpContext {
          Path        Protocol                                                                              Request: {
POST   /Home/Privacy   HTTP/1.1                     1st part                                                    Method: "POST"  
Host: localhost:1234                                2nd part: Headers                                           Path: "/Home/Privacy"
Connection: close                                                                                               Headers: [ {Connection: close }], ...
Authorization: Basic 2esdweyewt324=                                                                             Body: "x=10&y=20    
Content-Type: application/x-www-form-urlencoded     Required if there is a body                              }
                                                                                                             Response: {...}
x=10&y=20                                           3rd part: Body                                           User: null
                                                                                                             Session: null
                                                                                                             WebSocket: null
                                                                                                            }
                                                                                                             | Middleware
                                                                                                            userSession
                                                                                                             |
                                                                                                             HttpContext.Session = ...
                                                                                                             |
                                                                                                            AuthSessionMiddleware
                                                                                                             |
                                                                                                             HttpContext.User = HttpContext.Session[]
                                                                                                             |
                                                                                                            return Redirect()
                                                                                                             |
                                                                                                             HttpContext.Response:{
                                                                                                                     StatusCode: 302
                                                                                                                     Location: "/Home/"
                                                                                                             }
                                                                                                             [WebServer: IIS/Kestrel]
              <---------------------------------------------------------------------------------------------
                                                HTTP/1.1 302 Found
                                                Connection: close
                                                Location: "/Home"
                                                Server: "Kestrel"



*/                      