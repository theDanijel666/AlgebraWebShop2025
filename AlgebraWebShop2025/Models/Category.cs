using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2025.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(200,MinimumLength = 2)]
        public string Title { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        //Hijerarhijski pristup izrade kategorija

        //public int ParentCategoryId { get; set; }

        //[ForeignKey("ParentCategoryId")]
        //public Category ParentCategory { get; set; }
    }
}
