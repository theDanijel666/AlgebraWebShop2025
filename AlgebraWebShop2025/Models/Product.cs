using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2025.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200,MinimumLength = 2)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal Discount { get; set; }

        
        [ForeignKey("ProductId")]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        [ForeignKey("ProductId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        [ForeignKey("ProductId")]
        public virtual ICollection<Image> Images { get; set; }

    }
}
