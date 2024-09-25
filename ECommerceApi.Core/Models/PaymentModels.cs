using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Core.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public string Type { get; set; }
        public string InvoiceAddress { get; set; }
        public string ShippedAddress { get; set; }
        public bool IsCompleted { get; set; }
        public int? CartId { get; set; }
        public int? AccountId { get; set; }
    }


    public class PayModel
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }
        [Required]
        [StringLength(40)]
        public string CardName { get; set; }
        [Required]
        [StringLength (5)]
        [RegularExpression(@"^\d{2}\/\d{2}$")]
        public string ExpireDate { get; set; }
        [Required]
        [StringLength(3)]
        [RegularExpression(@"^\d{3}$")]
        public string CVV { get; set; }
        public decimal? TotalPriceOverride { get; set; }
        [StringLength(25)]
        public string Type { get; set; }
        [Required]
        [StringLength(160)]
        public string InvoiceAddress { get; set; }
        [Required]
        [StringLength(160)]
        public string ShippedAddress { get; set; }
    }
}
