using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Max 50 tecken")]
        public string Name { get; set; }
        [Range(0, 1000000)]
        public int Price { get; set; }
        [Display(Name = "Order date")]
        [DataType(DataType.Date)]
        public DateTime Orderdate { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Shelf { get; set; }
        [Range(0, 1000000)]
        public int Count { get; set; }
        [StringLength(100, ErrorMessage = "Max 100 tecken")]
        public string Description { get; set; }

    }
}
