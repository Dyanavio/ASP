using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP.Filters
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        override public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            if(context.HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new JsonResult(new
                {
                    status = 401,
                    message = "Unauthorized"
                });
            }

            //Console.ForegroundColor = ConsoleColor.DarkCyan;
            //Console.WriteLine("OnActionExecuting");
            //Console.ResetColor();

            
        }

        override public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
            base.OnActionExecuted(context);
        }
    }
}
