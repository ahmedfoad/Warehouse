using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class Srch_Salary
    {
        public string Bian { get;  set; }
        public string Date_Invoice_From { get;  set; }
        public string Date_Invoice_TO { get;  set; }
        public int ID_From { get;  set; }
        public int ID_To { get;  set; }
        public int Money_From { get;  set; }
        public int Money_To { get;  set; }
    }
}