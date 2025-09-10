using ASP.Data.Entities;
using ASP.Models.Rest;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(ILogger<CartController> logger, DataAccessor dataAccessor) : ControllerBase
    {
        private readonly ILogger<CartController> _logger = logger;
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpPost("{id}")]
        public RestResponse AddToCart([FromRoute] string id)
        {
            RestResponse response = new();
            response.Meta.ResourceName = "Shop API 'cart'";
            response.Meta.ResourceUrl = $"/api/cart/{id}";
            response.Meta.Method = "POST";
            response.Meta.Manipulations = ["POST", "PATCH", "DELETE"];

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
                        _dataAccessor.AddToCart(userId, id);
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
                        _logger.LogError("AddToCart {e}", e.Message);
                    }
            }
            else
            {
                response.Status = RestStatus.RestStatus401;
            }
            return response;
        }

        [HttpPatch("{id}")]
        public RestResponse ChangeCart(string id, int increment)
        {
            RestResponse response = new();
            response.Meta.ResourceName = "Shop API 'cart'";
            response.Meta.ResourceUrl = $"/api/cart/{id}";
            response.Meta.Method = "PATCH";
            response.Meta.Manipulations = ["POST", "PATCH", "DELETE"];
            response.Data = increment;

            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;
                if (userId == null)
                {
                    response.Status = RestStatus.RestStatus403;
                    response.Data = "PrimarySid was not found";
                    response.Meta.DataType = "string";
                }
                else 
                    try
                    {
                        _dataAccessor.ModifyCart(userId, id, increment);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        response.Status = RestStatus.RestStatus409;
                        response.Data = "Incremenet too large. Out of stock";
                        response.Meta.DataType = "string";
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
                        _logger.LogError("AddToCart {e}", e.Message);
                    }
            }
            else
            {
                response.Status = RestStatus.RestStatus401;
            }

            return response;
        }

        [HttpDelete("{id}")]
        public RestResponse RemoveFromCart(string id)
        {
            RestResponse response = new();
            response.Meta.ResourceName = "Shop API 'cart'";
            response.Meta.ResourceUrl = $"/api/cart/{id}";
            response.Meta.Method = "DELETE";
            response.Meta.Manipulations = ["DELETE"];

            if(HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;
                if(userId == null)
                {
                    response.Status = RestStatus.RestStatus403;
                    response.Data = "PrimarySid was not found";
                    response.Meta.DataType = "string";
                }
                else
                {
                    try
                    {
                        _dataAccessor.RemoveFromCart(userId, id);
                    }
                    catch(Exception e) when (e is ArgumentException || e is ArgumentNullException || e is FormatException || e is InvalidDataException)
                    {
                        response.Status = RestStatus.RestStatus400;
                        response.Data = e.Message;
                        response.Meta.DataType = "string";
                    }
                    catch(Exception e)
                    {
                        response.Status = RestStatus.RestStatus500;
                        _logger.LogError("RemoveFromCart: {e}", e.Message);
                    }
                }
            }
            else
            {
                response.Status = RestStatus.RestStatus401;
            }

            return response;

        }
    }
}
