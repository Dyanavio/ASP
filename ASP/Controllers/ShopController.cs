using ASP.Data.Entities;
using ASP.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        public IActionResult Cart(string? id)
        {
            ShopCartPageModel model = new();
            if(HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                string? userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)!.Value;
                if(id == null)
                {
                    model.ActiveCartItems = _dataAccessor.GetActiveCartItems(userId);
                }
                else
                {
                    Data.Entities.Cart? cart;
                    try
                    {
                        cart = _dataAccessor.GetCartById(id);
                    }
                    catch { cart = null; }
                    model.ActiveCartItems = cart?.CartItems;
                    model.IsActive = cart != null
                        && cart.PaidAt == null
                        && cart.DeletedAt == null;
                }
            }
            
            return View(model);
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
