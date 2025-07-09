namespace ASP.Data.Entities
{
    public class UserRole
    {
        public string Id { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool CanCreate { get; set; } 
        public bool CanRead { get; set; }   
        public bool CanUpdate { get; set; } 
        public bool CanDelete { get; set; } 
        public List<UserAccess> UserAccesses { get; set; } = [];
    }
}
