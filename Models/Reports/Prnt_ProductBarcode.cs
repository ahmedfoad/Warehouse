using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models.Reports
{
    public class Prnt_ProductBarcode
    {
        public string Name { get; set; }


        public string Price_Unit { get; set; }
        public string Taxes_Price { get; set; }
        public string Taxes { get; set; }
        public string TotalPrice { get; set; }
        public string Barcode { get; set; }


        public int Company_Id { get; set; }
        public string Company_Name { get; set; }
        public string User_Name { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
      
      
      
        public string Price_Mowrid { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        
        public byte[] BarCodeArr { get; set; }
        public System.Drawing.Image BarCodeImg { get; set; }

    }
}