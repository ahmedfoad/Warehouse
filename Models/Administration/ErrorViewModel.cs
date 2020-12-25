using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;

namespace Warehouse.Models.Administration
{
    public class ErrorViewModel
    {
        public string Url { get; set; }
        public string ErrorName { get; set; }
        public string ErrorNumber { get; set; }
        public string ErrorFullNumber { get; set; }

        public string fileName { get; set; }
        public int ID { get; set; }
        public int Invoice_Product_ID { get; set; }
        public int index { get; set; }
        public DateTime Date_Added { get; set; }
        public int Sadad_Type_Id { get;  set; }
        public int Invoice_Number { get;  set; }
        public decimal Omola_Money { get;  set; }
        // public int Invoice_Mandob_Sadad_ID { get; set; }
        public int Sadad_ID { get; set; }
        public decimal Total_Sadad { get;  set; }
    }
}