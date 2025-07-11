﻿namespace ASP.Data.Entities
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime? Birthdate { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<UserAccess> UserAccesses { get; set; } = [];
    }
}
