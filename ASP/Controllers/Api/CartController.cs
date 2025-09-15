using ASP.Data.Entities;
using ASP.Filters;
using ASP.Models.Rest;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Reflection;

namespace ASP.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    [RestFilter(Name: "Shop API 'user cart'")]
    public class CartController(ILogger<CartController> logger, DataAccessor dataAccessor) : ControllerBase
    {
        private readonly ILogger<CartController> _logger = logger;
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private RestResponse response = null!;

        [HttpDelete]
        public RestResponse DiscardCart()
        {
            response.Meta.ResourceUrl = $"/api/cart/";
            response.Meta.Manipulations = ["PUT", "DELETE"];

            string methodName = MethodBase.GetCurrentMethod()!.Name;
            ExecuteAuthority(_dataAccessor.DiscardActiveCart, nameof(methodName));

            return response;
        }

        [HttpPut]
        public RestResponse CheckoutCart()
        {
            response.Meta.ResourceUrl = $"/api/cart/";
            response.Meta.Manipulations = ["PUT", "DELETE"];

            //string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;
            //Cart? cart = _dataAccessor.GetActiveCart(userId);
            //
            //response.Data = new
            //{
            //    Price = cart.Price,
            //    Quantity = cart.CartItems.Count,
            //};

            string methodName = MethodBase.GetCurrentMethod()!.Name;
            ExecuteAuthority(_dataAccessor.CheckoutActiveCart, nameof(methodName));

            return response;
        }

        [HttpPost("{id}")]
        public RestResponse AddToCart([FromRoute] string id)
        {
            response.Meta.ResourceUrl = $"/api/cart/{id}";
            response.Meta.Manipulations = ["POST", "PATCH", "DELETE"];
            string methodName = MethodBase.GetCurrentMethod()!.Name;

            ExecuteAuthority((userId) => _dataAccessor.AddToCart(userId, id), nameof(methodName));

            return response;
        }

        [HttpPatch("{id}")]
        public RestResponse ChangeCart(string id, int increment)
        {
            response.Meta.ResourceUrl = $"/api/cart/{id}";
            response.Meta.Manipulations = ["POST", "PATCH", "DELETE"];
            response.Data = increment;
            string methodName = MethodBase.GetCurrentMethod()!.Name;

            ExecuteAuthority((userId) => _dataAccessor.ModifyCart(userId, id, increment), nameof(methodName));

            return response;
        }

        [HttpDelete("{id}")]
        public RestResponse RemoveFromCart(string id)
        {
            response.Meta.ResourceUrl = $"/api/cart/{id}";
            response.Meta.Manipulations = ["DELETE"];

            string methodName = MethodBase.GetCurrentMethod()!.Name;

            ExecuteAuthority((userId) => _dataAccessor.RemoveFromCart(userId, id), nameof(methodName));

            return response;
        }


        [HttpPost("repeat/{id}")]
        public RestResponse RepeatCart([FromRoute] string id)
        {
            //response.Meta.ResourceUrl = $"/api/cart/repeat/{id}";
            //response.Meta.Manipulations = ["POST", "PATCH", "DELETE"];
            //string methodName = MethodBase.GetCurrentMethod()!.Name;
            //
            //ExecuteAuthority((userId) => _dataAccessor.AddToCart(userId, id), nameof(methodName));

            response.Data = id;
            return response;
        }

        private void ExecuteAuthority(Action<string> action, string caller)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;
                if (userId == null)
                {
                    response.Status = RestStatus.RestStatus403;
                    response.Data = "PrimarySid was not found";
                    response.Meta.DataType = "string";
                }
                else try
                    {
                        action.Invoke(userId);
                        // or action(userId)
                    }
                    catch (Exception e) when (e is ArgumentException || e is ArgumentNullException || e is FormatException)
                    {
                        response.Status = RestStatus.RestStatus400;
                        response.Data = e.Message;
                        response.Meta.DataType = "string";
                    }
                    catch (Exception e)
                    {
                        response.Status = RestStatus.RestStatus500;
                        _logger.LogError("{caller} | {e}", caller, e.Message);
                    }
            }
            else
            {
                response.Status = RestStatus.RestStatus401;
            }
        }
    }



}
