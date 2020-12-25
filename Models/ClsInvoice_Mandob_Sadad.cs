using System;
using System.Collections.Generic;

namespace Warehouse.Models
{ 
    public class ClsInvoice_Mandob_Sadad
    {
        public DateTime Date_Added { get; set; }
        public int ID { get; set; }
        public int Invoice_Id { get; set; }
        public decimal Money { get; set; }
        public decimal Remain { get; set; }
        public int Sadad_Type_Id { get; set; }
        //public List<ClsInvoice_Mandob_Sadad_Rproduct> ClsInvoice_Mandob_Sadad_Rproduct { get; set; }
    }

    //public class ClsInvoice_Mandob_Sadad_Rproduct
    //{
    //    public int ID { get; set; }
    //    public int Invoice_Mandob_Sadad_id { get; set; }
    //    public int Return_Invoice_Product_id { get; set; }
    //    public string Product_Name { get; set; }
    //    public int Amount { get; set; }
    //    public decimal Price { get; set; }
    //    public DateTime InDate { get; set; }
    //    public DateTime EditDate { get; set; }
    //}
}