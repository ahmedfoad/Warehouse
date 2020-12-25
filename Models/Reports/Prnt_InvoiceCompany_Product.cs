using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class Prnt_InvoiceCompany_Product
    {
        public string Invoice_Number { get; set; }
        public string Nakl_Cost { get; set; }
        public string Company_Name { get; set; }
        public string Date_Invoice { get; set; }
        public string Time_Invoice { get; set; }
        public string Price { get; set; }
        public string Taxes { get; set; }
        public string TotalPrice { get; set; }
        public string Product_Name { get; set; }
        public string Product_Amount { get; set; }
        public string Product_Price { get; set; }
        public string Product_Taxes { get; set; }
        public string Product_TotalPrice { get; set; }
        public string TotalPrice_Tafkeet { get;  set; }
        public string Warehouse_Address { get; set; }
        public string Warehouse_Mobile { get; set; }
        public string Warehouse_Email { get; set; }
    }
}