namespace ASP.Data.Entities
{
    public class ProductGroup
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime? DeletedAt { get; set; }

        // Navigation properties - are properties of Entity type
        public ProductGroup? ParentGroup { get; set; }
        public ICollection<Product> Products { get; set; } = [];

    }
}
