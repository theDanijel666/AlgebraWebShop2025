using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2025.Models
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        public string ProductTitle { get; set; }

        [NotMapped]
        public string CategoryTitle { get; set; }
    }
}
