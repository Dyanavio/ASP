using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Api.Group
{
    public class ApiGroupFormModel
    {
        [FromForm(Name = "group-name")]
        public string? Name { get; set; } = null!;

        [FromForm(Name = "group-description")]
        public string? Description { get; set; } = null!;

        [FromForm(Name = "group-slug")]
        public string? Slug { get; set; } = null!;

        [FromForm(Name = "group-parent")]
        public string? ParentId { get; set; }

        [FromForm(Name = "group-image")]
        public IFormFile? Image { get; set; } = null!;
    }
}
