
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Task.Data
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; } 

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public Product? Product { get; set; }
    }
}
