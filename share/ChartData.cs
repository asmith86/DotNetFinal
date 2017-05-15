using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Northwind.Models
{
    public class ChartData
    {
        public string company_name { get; set; }
        public string present_worth { get; set; }

        public ChartData(string name, string worth)
        {
            company_name = name;
            present_worth = worth;
        }
    }
}