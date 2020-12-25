using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class ClsProduct
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price_Unit { get; set; }
        public decimal Taxes_Price { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalPrice { get; set; }
        public int OldAmount { get; set; }

        public string Barcode { get; set; }
        public int Company_Id { get; set; }
        public string Company_Name { get; set; }
        public string User_Name { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }


        public decimal Price_Mowrid { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }


        public Nullable<int> Offer_TargetAmount { get; set; }
        public Nullable<int> Offer_BonusAmount { get; set; }
        public Nullable<int> Offer_Product_id { get; set; }
        public string Offer_Product_Name { get; set; }
        public byte[] BarCodeArr { get; set; }
        public System.Drawing.Image BarCodeImg { get; set; }
        public decimal Price_Mowrid_Copy { get;  set; }

        public Nullable<decimal> Shop_Price { get; set; }
        public Nullable<int> Shop_Offer_TargetAmount { get; set; }
        public Nullable<int> Shop_Offer_BonusAmount { get; set; }
        public Nullable<int> Shop_Offer_Product_id { get; set; }
        public string Shop_Offer_Product_Name { get; set; }
        public Nullable<decimal> Home_Price { get; set; }
        public Nullable<int> Home_TargetAmount { get; set; }
        public Nullable<int> Home_Offer_BonusAmount { get; set; }
        public Nullable<int> Home_Offer_Product_id { get; set; }
        public string Home_Offer_Product_Name { get; set; }

    }


}