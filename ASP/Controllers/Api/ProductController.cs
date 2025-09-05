using ASP.Models.Api.Product;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using ASP.Filters;
using ASP.Data.Entities;
using ASP.Models.Rest;

namespace ASP.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizationFilter]
    public class ProductController(ILogger<ProductController> logger,
                                   IStorageService storageService, 
                                   DataAccessor dataAccessor) : ControllerBase
    {
        private readonly ILogger<ProductController> _logger = logger;
        private readonly IStorageService _storageService = storageService;
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpGet]
        public IEnumerable<string> ProductsList()
        {
            return ["1", "2", "3"];
        }

        [HttpPost]
        public async Task<RestResponse> CreateProduct(ApiProductFormModel formModel)
        {
            RestResponse response = new();
            response.Meta.ResourceName = "Shop API 'product'";
            response.Meta.Method = "POST";
            response.Meta.Manipulations = ["GET", "POST", "PATCH", "DELETE"];

            //VALIDATION
            if (string.IsNullOrEmpty(formModel.Name))
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = "Name must not be empty";
                response.Meta.DataType = "string";
            }
            else
            {
                if (_dataAccessor.IsProductNameUsed(formModel.Name))
                {
                    response.Status = RestStatus.RestStatus400;
                    response.Data = "Name is already used";
                    response.Meta.DataType = "string";
                }
            }
            

            if (!string.IsNullOrEmpty(formModel.Slug))
            {
                if (_dataAccessor.IsProductSlugUsed(formModel.Slug))
                {
                    response.Status.StatusCode = 409;
                    response.Status.StatusMessage = "Conflict";
                    response.Status.IsOk = false;
                    response.Data = "Slug is already used";
                    response.Meta.DataType = "string";
                }
            }

            if(formModel.Price != null)
            {
                if (double.IsNaN((double)formModel.Price))
                {
                    response.Status = RestStatus.RestStatus400;
                    response.Data = "Price is not a propert number";
                    response.Meta.DataType = "string";
                }
            }
            else
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = "Price must not be empty";
                response.Meta.DataType = "string";
            }

            if (formModel.Stock != null)
            {
                if (double.IsNaN((int)formModel.Stock))
                {
                    response.Status = RestStatus.RestStatus400;
                    response.Data = "Stock number/amount is not a propert value";
                    response.Meta.DataType = "string";
                }
            }
            else
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = "Stock number/amount must not be empty";
                response.Meta.DataType = "string";
            }

            string? savedName = null;
            if(formModel.Image != null)
            {
                try
                {
                    // Checking the extension
                    _storageService.TryGetMimeType(formModel.Image.FileName);
                    savedName = await _storageService.SaveItemAsync(formModel.Image);
                }
                catch (Exception e)
                {
                    response.Status = RestStatus.RestStatus400;
                    response.Data = e.Message;
                    response.Meta.DataType = "string";
                }
            }
            try
            {
                _dataAccessor.AddProduct(new()
                {
                    Name = formModel.Name!,
                    GroupId = formModel?.GroupId!,
                    Description = formModel?.Description!,
                    Slug = formModel?.Slug,
                    Price = (double)formModel!.Price!,
                    Stock = (int)formModel!.Stock!,
                    ImageUrl = savedName
                });
                response.Status.StatusCode = 201;
                response.Status.StatusMessage = "Created";
                response.Meta.DataType = "string";
                response.Data = "Created";
            }
            catch(Exception e) when (e is ArgumentNullException || e is FormatException)
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = e.Message;
                response.Meta.DataType = "string";
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                response.Status = RestStatus.RestStatus500;
                response.Data = e.Message;
                response.Meta.DataType = "string";
            }
            return response;
        }
    }
}

/*

Differences between MVC and API controllers 

MVC: one method (usually GET) and different addresses 
(You can reach ONE address with ONE method, action is determined by address)
GET /home/privacy -> HomeController::Privacy()
POST /home/index -> HomeController::Index()   (Post makes no difference, we will end up on Index)

API: one address, but different methods
GET  /api/product -> ProductController::ProductsList()
POST /api/product -> ProductController::CreateProduct()
PUT  /api/product

-----------------------------------------------------------------------------------------

MVC - returns IActionResult
API - returns objects of an arbitrary type that ASP changes them to JSON (except for string, it changes to plain/text)

 */