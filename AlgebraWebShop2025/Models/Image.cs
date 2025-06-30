using System.ComponentModel.DataAnnotations;

namespace AlgebraWebShop2025.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        public bool IsMain { get; set; }

        [Required]
        [StringLength(100,MinimumLength = 2)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(250)]
        public string URL { get; set; }

        public int ProductId { get; set; }
    }
}
