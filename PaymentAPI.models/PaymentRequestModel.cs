using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.models
{
    public class PaymentRequestModel
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }
        [Required]
        [StringLength(40)]
        public string CardName { get; set; }
        [Required]
        [StringLength(5)]
        [RegularExpression(@"^\d{2}\/\d{2}$")]
        public string ExpireDate { get; set; }
        [Required]
        [StringLength(3)]
        [RegularExpression(@"^\d{3}$")]
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
