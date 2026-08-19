using System.ComponentModel.DataAnnotations;
namespace Final_Task.Data
{
    public class Product
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}

