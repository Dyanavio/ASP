using ASP.Data.Entities;

namespace ASP.Models.User
{
    public class UserProfilePageModel
    {
        public bool? IsPersonal { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public IEnumerable<Cart> Carts { get; set; } = [];
        
    }
}
