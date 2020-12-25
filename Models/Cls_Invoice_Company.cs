using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;

namespace Warehouse.Models
{
    public class Cls_Invoice_Company
    {
        public int ID { get; set; }
        public int Company_id { get; set; }
        public string Company_Name { get; set; }
        public string Date_Invoice { get; set; }
        public DateTime _Date_Invoice { get; set; }
        public string Date_Invoice_Hijri { get; set; }
        public decimal Price { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total_Sadad { get; set; }
        public int User_ID { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public string InDate { get; set; }
     
        public List<ClsInvoiceCompany_Product> ClsInvoiceCompany_Product { get; set; }
        public List<ClsInvoice_Company_Sadad> ClsInvoice_Company_Sadad { get; set; }
        public User User { get;  set; }
       
        public decimal Remain { get; set; }
        public int Sadad_Count { get; set; }
        //public decimal TotalPrice { get;  set; } //خطأ لا يوجد
        public decimal Nakl_Cost { get;  set; }
        public int Invoice_Number { get;  set; }
    }

    public partial class ClsInvoiceCompany_Product
    {
        public int ID { get; set; }
        public int Invoice_Company_Id { get; set; }
        public int Product_Id { get; set; }
        public string Product_Name { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalPrice { get; set; }
     

    }
    public partial class ClsInvoice_Company_Sadad
    {
        public int ID { get; set; }
        public int Invoice_Id { get; set; }
        public int Sadad_Type_Id { get; set; }
        public DateTime Date_Added { get; set; }
        public decimal Money { get; set; }
        public decimal Remain { get; set; }
      
       
    }
}