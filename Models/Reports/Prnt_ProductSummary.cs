using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models.Reports
{
    public class Prnt_ProductSummary
    {
        public string Warehouse_Address { get; set; }
        public string Warehouse_Mobile { get; set; }
        public string Warehouse_Email { get; set; }
        public string Product_Name { get; set; }
        public string Company_Name { get; set; }
        public string Prev_Amount { get; set; }
        public string Sell_Amount { get; set; }
        public string Current_Amount { get; set; }

    }

   
}