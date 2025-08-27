using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        private readonly IStorageService _storageService = storageService;
        
        [HttpGet]
        public IActionResult Item(string id)
        {
            try
            {
                return File(_storageService.GetItemBytes(id), _storageService.TryGetMimeType(id));
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
