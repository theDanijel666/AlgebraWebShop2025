using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2025.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal Quantity {  get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal Discount { get; set; }

        [NotMapped]
        public string ProductTitle { get; set; }
    }
}
