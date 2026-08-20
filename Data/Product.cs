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
        
        public double Price { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}

