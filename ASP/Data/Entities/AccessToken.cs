using System.Text.Json.Serialization;

namespace ASP.Data.Entities
{
    public class AccessToken // organized by JWT standard
    {
        public string Jti { get; set; } = null!;
        public Guid? Sub { get; set; }  // UserAccessId
        public string? Iat { get; set; }
        public string? Exp { get; set; }
        public string? Nbf { get; set; }
        public string? Aud { get; set; } // Role / RoleId
        public string? Iss { get; set; } // We are the issuer

        [JsonIgnore]
        public UserAccess UserAccess { get; set; } = null!;

    }
}
