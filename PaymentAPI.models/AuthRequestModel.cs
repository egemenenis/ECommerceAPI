using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.models
{
    public class AuthRequestModel
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
