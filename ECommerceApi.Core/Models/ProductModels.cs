using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Core.Models
{
   public class ProductCreateModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool Discountinued { get; set; }
        public int CategoryId { get; set; }
    }


    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool Discountinued { get; set; }
        public int CategoryId { get; set; }
        public int AccountId { get; set; }
        public string CategoryName { get; set; }
        public string AccountCompanyName { get; set; }
    }
}
