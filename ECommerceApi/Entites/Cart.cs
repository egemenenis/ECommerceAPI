using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApi.API.Entites
{
    [Table("Carts")]
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsClosed { get; set; }
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
        public virtual List<CartProduct> CartProducts { get; set; }
        public Cart()
        {
            CartProducts = new List<CartProduct>(); 
        }
    }
}
