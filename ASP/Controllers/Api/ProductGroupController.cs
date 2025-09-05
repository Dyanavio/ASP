using ASP.Data.Entities;
using ASP.Models.Api.Group;
using ASP.Models.Rest;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASP.Controllers.Api
{
    [Route("api/product-group")]
    [ApiController]
    public class ProductGroupController(ILogger<ProductGroupController> logger, DataAccessor dataAccessor, IStorageService storageService) : ControllerBase
    {

        private readonly ILogger<ProductGroupController> _logger = logger;
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IStorageService _storageService = storageService;

        private object AnyRequest()
        {
            string methodName = "Execute" + HttpContext.Request.Method;
            var type = GetType();
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
        public RestResponse ExecutePOST(ApiGroupFormModel formModel)
        {
            RestResponse response = new();
            response.Meta.ResourceName = "Shop API 'product-group'";
            response.Meta.Method = "POST";
            response.Meta.Manipulations = ["GET", "POST", "PATCH", "DELETE"];

            // VALIDATION
            if (string.IsNullOrEmpty(formModel.Name))
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = "Name must not be empty";
                response.Meta.DataType = "string";
            }
            else
            {
                if (_dataAccessor.IsGroupNameUsed(formModel.Name))
                {
                    response.Status = RestStatus.RestStatus400;
                    response.Data = "Name is already used";
                    response.Meta.DataType = "string";
                }
            }

            if (string.IsNullOrEmpty(formModel.Description))
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = "Description must not be empty";
                response.Meta.DataType = "string";
            }
            
            if (string.IsNullOrEmpty(formModel.Slug))
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = "Slug must not be empty";
                response.Meta.DataType = "string";
            }
            else
            {
                if (_dataAccessor.IsGroupSlugUsed(formModel.Slug))
                {
                    response.Status = RestStatus.RestStatus400;
                    response.Data = "Slug is already used";
                    response.Meta.DataType = "string";
                }
            }
            if(formModel.ParentId != "None")
            {
                if (formModel.ParentId != null && !Guid.TryParse(formModel.ParentId, out Guid guid))
                {
                    response.Status = RestStatus.RestStatus400;
                    response.Data = "Invalid parent group id";
                    response.Meta.DataType = "string";
                }
            }
            

            string? savedName = "";
            try
            {
                // Checking the extension
                _storageService.TryGetMimeType(formModel?.Image?.FileName!);
                savedName = _storageService.SaveItem(formModel?.Image!);
            }
            catch (Exception e)
            {
                response.Status = RestStatus.RestStatus400;
                response.Data = e.Message;
                response.Meta.DataType = "string";
            }
            if(!string.IsNullOrEmpty(savedName))
            {
                try
                {
                    _dataAccessor.AddProductGroup(new()
                    {
                        Name = formModel?.Name!,
                        Description = formModel?.Description!,
                        Slug = formModel?.Slug!,
                        ParentId = formModel?.ParentId != "None" ? formModel?.ParentId : null,
                        ImageUrl = savedName
                    });
                    response.Status.StatusCode = 201;
                    response.Status.StatusMessage = "Created";
                    response.Meta.DataType = "string";
                    response.Data = "Created";
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    response.Status = RestStatus.RestStatus500;
                    response.Data = "Server error";
                    response.Meta.DataType = "string";
                }
            }
            return response;
        }
    }
}



