using System.ComponentModel.DataAnnotations;

namespace Avacado.Services.ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(1,1000)]
        public double Price { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
		public string? ImageLocalPath { get; set; }
	}
}
