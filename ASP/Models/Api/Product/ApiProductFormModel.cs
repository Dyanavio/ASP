using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Api.Product
{
    public class ApiProductFormModel
    {
        [FromForm(Name = "product-name")]
        public string? Name { get; set; }

        [FromForm(Name = "product-group")]
        public string? GroupId { get; set; }

        [FromForm(Name = "product-description")]
        public string? Description { get; set; }

        [FromForm(Name = "product-slug")]
        public string? Slug { get; set; } 

        [FromForm(Name = "product-image")]
        public IFormFile? Image { get; set; }

        [FromForm(Name = "product-price")]
        public double? Price { get; set; }

        [FromForm(Name = "product-stock")]
        public int? Stock { get; set; }

    }
}
