using AlgebraWebShop2025.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2025.Data
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; } 

        [StringLength(200)]
        public string? Address { get; set; } 

        [StringLength(100)]
        public string? City { get; set; } 

        [StringLength(20)]
        public string? ZIP { get; set; } 

        [StringLength(100)]
        public string? Country { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
