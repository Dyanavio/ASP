using ASP.Data.Entities;
using ASP.Models.Api.Group;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers
{
    [Route("api/product-group")]
    [ApiController]
    public class ProductGroupController(DataAccessor dataAccessor, IStorageService storageService) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IStorageService _storageService = storageService;

        private object AnyRequest()
        {
            string methodName = "Execute" + HttpContext.Request.Method;
            var type = this.GetType();
            var action = type.GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (action == null)
            {
                return new
                {
                    status = 405,
                    message = "Method Not Allowed"
                };
            }
            if (HttpContext.Request.Method == "GET")
            {
                return action.Invoke(this, null)!;
            }
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if(!isAuthenticated)
            {
                return new
                {
                    status = 401,
                    message = "Unauthorized"
                };
            }
            return action.Invoke(this, null)!;
        }

        [HttpGet]
        public IEnumerable<ProductGroup> ExecuteGET()
        {
            return _dataAccessor.GetProductGroups();
        }

        [HttpPost]
        public object ExecutePOST(ApiGroupFormModel formModel)
        {
            // VALIDATION
            if (string.IsNullOrEmpty(formModel.Name))
            {
                return new { status = 400, name = "Name must not be empty" };
            }
            if(_dataAccessor.IsGroupNameUsed(formModel.Name))
            {
                return new { status = 400, name = "Name is already used" };
            }

            if (string.IsNullOrEmpty(formModel.Description))
            {
                return new { status = 400, name = "Description must not be empty" };
            }
            
            if (string.IsNullOrEmpty(formModel.Slug))
            {
                return new { status = 400, name = "Slug must not be empty" };
            }
            if (_dataAccessor.IsGroupSlugUsed(formModel.Slug))
            {
                return new { status = 400, name = "Slug is already used" };
            }

            if (formModel.ParentId != null && !(Guid.TryParse(formModel.ParentId, out Guid guid)))
            {
                return new { status = 400, name = "Invalid parent group id" };
            }

            string savedName;
            try
            {
                // Checking the extension
                _storageService.TryGetMimeType(formModel?.Image?.FileName!);
                savedName = _storageService.SaveItem(formModel?.Image!);
            }
            catch (Exception e)
            {
                return new { status = 400, name = e.Message };
            }
            try
            {
                _dataAccessor.AddProductGroup(new()
                {
                    Name = formModel?.Name!,
                    Description = formModel?.Description!,
                    Slug = formModel?.Slug!,
                    ParentId = formModel?.ParentId,
                    ImageUrl = savedName
                });
                return new { status = 201, name = "Created" };
            }
            catch(Exception)
            {
                return new { status = 500, name = "Server error" };
            }
            
        }
    }
}



