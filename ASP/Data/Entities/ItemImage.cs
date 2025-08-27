namespace ASP.Data.Entities
{
    public class ItemImage
    {
        public Guid ItemId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int Order { get; set; }
    }
}
