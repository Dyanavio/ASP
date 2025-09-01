using ASP.Data.Entities;

namespace ASP.Models.Shop
{
    public class ShopGroupPageModel
    {
        public ProductGroup? ProductGroup { get; set; } = null!;
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
