using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;

namespace Warehouse.Models
{
    public class Cls_Invoice_Mandob
    {
        public Cls_Invoice_Mandob()
        {
            ClsInvoiceMandob_Product = new List<ClsInvoiceMandob_Product>();
        }
        public int ID { get; set; }
        public int Customer_Type { get; set; }
        public string Customer_Name { get; set; }
        public int Invoice_Number { get; set; }
        public int Payment_Type { get; set; }
        public string Payment_Type_Name { get; set; }
        public Nullable<int> Mandob_id { get; set; }
        public string Mandob_Name { get; set; }
        public string Date_Invoice { get; set; }
        public DateTime _Date_Invoice { get; set; }
        public string Date_Invoice_Hijri { get; set; }
        public decimal Price { get; set; }
       // public decimal Taxes { get; set; }
        //public decimal TotalPrice { get; set; }
        public decimal Total_Sadad { get; set; }
        public int User_ID { get; set; }
        public string User_EmpNAME { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public string InDate { get; set; }
      
        public IList<ClsInvoiceMandob_Product> ClsInvoiceMandob_Product { get; set; }
        public decimal Remain { get; set; }
        public int Sadad_Count { get; set; }
        public List<ClsInvoice_Mandob_Sadad> ClsInvoice_Mandob_Sadad { get; set; }
      
        //***************************************************************
        public int Omola_Amount_AllProducts { get; set; }
        public bool Omola_Type { get; set; }
        public decimal Omola_quota { get; set; }
        public string Omola_Text { get; set; }
        public decimal Omola_Money { get; set; }
        public decimal Omola_Money_Orignal { get; set; }
        public decimal Total_Return { get;  set; }
        //***************************************************************

    }

    public  class ClsInvoiceMandob_Product
    {
        public int ID { get; set; }
        public int Invoice_Mandob_Id { get; set; }
        public int Product_Id { get; set; }
        public string Product_Name { get; set; }
        public string Product_Name_Orginal { get; set; }
        public int Amount { get; set; }
        //*******************************************
        public int Return_Amount { get; set; }
        public int Remain_Amount { get; set; }
        public decimal Return_Price { get; set; }
        //*******************************************
        public decimal Price { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalPrice { get; set; }
        public int? Offer_Product_id { get;  set; }
        public string Offer_Product_Name { get; set; }
        public int? Offer_BonusAmount { get;  set; }
        public int? Offer_Product_id_Orginal { get; set; }
        public string Offer_Product_Name_Orginal { get; set; }
        public int? Offer_BonusAmount_Orginal { get; set; }
        public decimal Price_Mowrid { get;  set; }
        //public int Invoice_Mandob_Sadad_id { get;  set; }
        //public int Return_Invoice_Product_id { get;  set; }
        public DateTime InDate { get;  set; }
    }
}