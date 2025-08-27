namespace ASP.Models.Api.Group
{
    public class ApiGroupDataModel
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? ParentId { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
