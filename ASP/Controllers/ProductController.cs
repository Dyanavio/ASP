using ASP.Models.Api.Product;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using ASP.Filters;

namespace ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizationFilter]
    public class ProductController(IStorageService storageService) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;

        [HttpGet]
        public IEnumerable<string> ProductsList()
        {
            return ["1", "2", "3"];
        }

        [HttpPost]
        public async Task<object> CreateProduct(ApiProductFormModel model)
        {
            string savedName;
            try
            {
                // Checking the extension
                _storageService.TryGetMimeType(model.Image.FileName);
                savedName = await _storageService.SaveItemAsync(model.Image);
            }
            catch(Exception e)
            {
                return new { status = "Fail", name = e.Message };
            }
            return new { status = "OK", name = savedName };
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