using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models { 
    public class Srch_Masrofat
    {
        public int ID_From { get; set; }
        public int ID_To { get; set; }
        public string Date_Invoice_From { get; set; }
        public string Date_Invoice_TO { get; set; }
        public int Masrofat_Type_Id { get; set; }
        public string Masrofat_Type_Name { get; set; }
        public decimal Money_From { get; set; }
        public decimal Money_To { get; set; }
        public string Bian { get; set; }
    }
}