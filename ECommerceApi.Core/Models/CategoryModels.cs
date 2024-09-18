using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Core.Models
{

    public class CategoryModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }



    public class CategoryCreateModel
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }



    public class CategoryUpdateModel
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
