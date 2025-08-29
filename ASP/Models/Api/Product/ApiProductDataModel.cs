namespace ASP.Models.Api.Product
{
    public class ApiProductDataModel
    {
        public string Name { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? ImageUrl { get; set; } 
        public int Stock { get; set; }
        public double Price { get; set; }
    }
}
