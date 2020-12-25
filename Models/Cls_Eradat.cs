using System;

namespace Warehouse.Models
{
    public class Cls_Eradat
    {
        public DateTime _Date_Invoice;

        public int ID { get; set; }
        public int Eradat_Type_Id { get; set; }
        public string Bian { get; set; }
        public string Notes { get; set; }
        public System.DateTime InDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public int Userid_In { get; set; }
        public Nullable<int> userid_Edit { get; set; }
        public string Date_Invoice { get; set; }
        public string Eradat_Type_Name { get;  set; }
        public decimal Money { get;  set; }
        public string User_Name { get;  set; }
        public string ComputerName { get;  set; }
        public string ComputerUser { get;  set; }
        public int Invoice_Number { get;  set; }
        public int IsEdit { get;  set; }
    }
}