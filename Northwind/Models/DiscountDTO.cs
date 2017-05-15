using System;

namespace Northwind.Models
{
    public class DiscountDTO
    {
        public int? Code { get; set; }
        public string Description { get; set; }
        public decimal? DiscountPercent { get; set; }
        public DateTime? EndTime { get; set; }
        public string Title { get; set; }
    }
}