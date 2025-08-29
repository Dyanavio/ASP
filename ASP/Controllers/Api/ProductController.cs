using ASP.Models.Api.Product;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using ASP.Filters;
using ASP.Data.Entities;

namespace ASP.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizationFilter]
    public class ProductController(IStorageService storageService, DataAccessor dataAccessor) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpGet]
        public IEnumerable<string> ProductsList()
        {
            return ["1", "2", "3"];
        }

        [HttpPost]
        public async Task<object> CreateProduct(ApiProductFormModel formModel)
        {
            //VALIDATION
            if (string.IsNullOrEmpty(formModel.Name))
            {
                return new { status = 400, name = "Name must not be empty" };
            }
            if (_dataAccessor.IsProductNameUsed(formModel.Name))
            {
                return new { status = 400, name = "Name is already used" };
            }

            if (!string.IsNullOrEmpty(formModel.Slug))
            {
                if (_dataAccessor.IsProductSlugUsed(formModel.Slug))
                {
                    return new { status = 409, name = "Slug is already used" };
                }
            }

            if(formModel.Price != null)
            {
                if (double.IsNaN((double)formModel.Price))
                {
                    return new { status = 400, name = "Price is not a propert number" };
                }
            }
            else
            {
                return new { status = 400, name = "Price must not be empty" };
            }

            if (formModel.Stock != null)
            {
                if (double.IsNaN((int)formModel.Stock))
                {
                    return new { status = 400, name = "Stock number/amount is not a propert value" };
                }
            }
            else
            {
                return new { status = 400, name = "Stock number/amount must not be empty" };
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
                    return new { status = 400, name = e.Message };
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
                return new { status = 201, name = "Created" };
            }
            catch(Exception e) when (e is ArgumentNullException || e is FormatException)
            {
                return new { status = 400, name = e.Message };
            }
            catch (Exception e)
            {
                return new { status = 500, name =e.Message };
            }
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