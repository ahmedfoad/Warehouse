using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models.Reports
{
    public class Prnt_Arbah
    {
        public string Index { get; set; }
        public string Warehouse_Address { get; set; }
        public string Warehouse_Mobile { get; set; }
        public string Warehouse_Email { get; set; }
        public string Date_Invoice_From { get; set; }
        public string Date_Invoice_TO { get; set; }
     
        public string Bian { get; set; }
        public string Date_Invoice { get; set; }
      
        public string Type_Name { get; set; }
        public string Money_Eradat { get; set; }
        public string Money_Masrofat { get; set; }
      
        public string Total_Eradat { get; set; }
        public string Total_Masrofat { get; set; }
        public string Total_Arbah { get; set; }
    }
}