using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class Cls_Arbah
    {
        public DateTime _Date_Invoice;

        public string Bian { get; set; }
        public string Notes { get; set; }
        public System.DateTime InDate { get; set; }
        public string Date_Invoice { get; set; }
        public string Type_Name { get; set; }
        public decimal Money_Eradat { get; set; }
        public decimal Money_Masrofat { get; set; }

    }
}