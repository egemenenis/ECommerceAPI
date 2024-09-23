using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Core.Models
{
    public class CartModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsClosed { get; set; }
        public int AccountId { get; set; }
    }
}
