using ASP.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpPost("{id}")]
        public object AddToCart([FromRoute] string id)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;
                if(userId == null)
                {
                    HttpContext.Response.StatusCode = 403;
                    return new { message = "Forbidden. PrimarySid was not found" };
                }
                try
                {
                    _dataAccessor.AddToCart(userId, id);
                    return new { message = "Ok" };
                }
                catch(Exception e) when (e is ArgumentNullException || e is FormatException)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new { message = e.Message };
                }
                catch
                {
                    HttpContext.Response.StatusCode = 500;
                    return new { message = "Internal Server Error" };
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 401;
                return new { message = "Unauthorized" };
            }
        }
    }
}
