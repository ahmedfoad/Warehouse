using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models.Reports
{
     

    public class Prnt_Invoice_Mandob
    {
        public string Warehouse_Address { get; set; }
        public string Warehouse_Mobile { get; set; }
        public string Warehouse_Email { get; set; }
        public string Product_Name { get; set; }
        public string Amount { get; set; }
        public string Price { get; set; }
        public string Taxes { get; set; }
        public string TotalPrice { get; set; }

        //**********************************************
        public string ID { get; set; }
        public Nullable<int> Mandob_id { get; set; }
        public string Mandob_Name { get; set; }
        public string Mandob_Mobile { get; set; }
        public string Date_Invoice { get; set; }
        public DateTime _Date_Invoice { get; set; }
        public string Date_Invoice_Hijri { get; set; }
        public string Price_Invoice { get; set; }
        public string Taxes_Invoice { get; set; }
        public string TotalPrice_Invoice { get; set; }
        public string Total_Sadad_Invoice { get; set; }
        public int User_ID { get; set; }
        public string User_EmpNAME { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public string InDate { get; set; }
       
        public String Time { get; set; }
        public string TotalPrice_Tafkeet { get;  set; }
    }
}
