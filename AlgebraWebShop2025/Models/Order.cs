using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2025.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Total price is required!")]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Total { get; set; }

        #region Billing info

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        public string BillingFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string BillingLastName { get; set; }


        [Required(ErrorMessage = "Email Adress is required")]
        [StringLength(150)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string BillingEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(100)]
        public string BillingPhone { get; set; }

        [Required(ErrorMessage = "Adress is required")]
        [StringLength(200)]
        public string BillingAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string BillingCity { get; set; }

        [Required(ErrorMessage = "ZIP is required")]
        [StringLength(20)]
        public string BillingZIP { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        public string BillingCountry { get; set; }

        #endregion

        #region Shipping info

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        public string ShippingFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string ShippingLastName { get; set; }


        [Required(ErrorMessage = "Email Adress is required")]
        [StringLength(150)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string ShippingEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(100)]
        public string ShippingPhone { get; set; }

        [Required(ErrorMessage = "Adress is required")]
        [StringLength(200)]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string ShippingCity { get; set; }

        [Required(ErrorMessage = "ZIP is required")]
        [StringLength(20)]
        public string ShippingZIP { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        public string ShippingCountry { get; set; }

        #endregion

        public string Message { get; set; }

        public string UserId {  get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}
