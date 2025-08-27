using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Api.Product
{
    public class ApiProductFormModel
    {
        [FromForm(Name = "product-name")]
        public string Name { get; set; } = null!;

        [FromForm(Name = "product-image")]
        public IFormFile Image { get; set; } = null!;
    }
}
