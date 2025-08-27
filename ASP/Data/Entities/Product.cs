using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid? GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Slug { get; set; } = null!;
        public string? ImageUrl { get; set; } = null!;

        [Column(TypeName = "decimal(12, 2)")]
        public double Price { get; set; }
        public int Stock { get; set; }
        public DateTime? DeletedAt { get; set; }


        public ProductGroup? Group { get; set; }
    }
}
