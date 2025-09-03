using ASP.Data.Entities;
using ASP.Models.Shop;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers
{
    public class ShopController(DataAccessor dataAccessor) : Controller
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        public IActionResult Index()
        {
            ShopIndexPageModel model = new()
            {
                ProductGroups = _dataAccessor.GetProductGroups()
            };

            return View(model);
        }

        public IActionResult Group([FromRoute]string id)
        {
            ShopGroupPageModel model = new()
            {
                ProductGroup = _dataAccessor.GetProductGroupBySlug(id),
                ProductGroups = _dataAccessor.GetProductGroups()
            };

            return View(model);
        }

        public IActionResult Item([FromRoute] string id)
        {
            ShopItemPageModel model = new()
            {
                Product = _dataAccessor.GetProductBySlug(id),
                ProductGroups = _dataAccessor.GetProductGroups()
            };
            return View(model);
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Admin()
        {
            ShopAdminPageModel model = new()
            {
                ProductGroups = _dataAccessor.GetProductGroups().Select(g => new Models.OptionModel()
                {
                    Value = g.Id.ToString(),
                    Content = $"{g.Name} ({g.Description})",
                })
            };
            return View(model);
        }
    }
}
