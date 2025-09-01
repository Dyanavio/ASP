using ASP.Data.Entities;

namespace ASP.Models.Shop
{
    public class ShopItemPageModel
    {
        public Product? Product { get; set; }
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
