using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Northwind.Models
{
    public class OrderDetailsDTO
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }


        
    }
}