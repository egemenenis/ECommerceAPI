using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApi.API.Entites
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(25)]
        public string Username { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        [StringLength(50)]
        public string? CompanyName { get; set; }
        [StringLength(50)]
        public string? ContactName { get; set; }
        [StringLength(50)]
        [EmailAddress]
        public string? ContactEmail { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsApplyment { get; set; }
        public AccountType Type { get; set; }
        public virtual List<Product> Products { get; set; }
        public virtual List<Cart> Carts { get; set; }
        public virtual List<Payment> Payments { get; set; }
    }

    public enum AccountType
    {
        Member,
        Admin,
        Merchant
    }
}
