using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;

namespace Warehouse.Models.Search
{
    public class Srch_Invoice_Mandob
    {
        public int Invoice_From { get; set; }
        public int Invoice_To { get; set; }
        public int Mandob_id { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int Price_From { get; set; }
        public int Price_To { get; set; }
    }
}