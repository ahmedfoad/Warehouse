using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class Cls_Masrofat
    {
        public int ID { get; set; }
        public int Masrofat_Type_Id { get; set; }
        public string Masrofat_Type_Name { get; set; }
        public string Bian { get; set; }
        public string Notes { get; set; }
        public System.DateTime InDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public int Userid_In { get; set; }
        public Nullable<int> userid_Edit { get; set; }
        public string Date_Invoice { get; set; }
        public DateTime _Date_Invoice { get; set; }
        public decimal Money { get;  set; }
        public object User_Name { get;  set; }
        public string ComputerName { get;  set; }
        public string ComputerUser { get;  set; }
        public int Invoice_Number { get;  set; }
        public Nullable<int> IsEdit { get; set; }
    }
}