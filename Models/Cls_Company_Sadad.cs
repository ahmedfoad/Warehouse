using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class Cls_Company_Sadad
    {
        public string ID { get; set; }
        public int Sadad_Type_Id { get; set; }
        public string Sadad_Type_Name { get; set; }
        public int User_Id { get; set; }
        public string User_Name { get; set; }
        public string Money { get; set; }
        public string Company_InvoicePrice { get; set; }
        public string Company_MoneySadad { get; set; }
        public string Company_Money_Remain { get; set; }
     
        public string InDate { get; set; }
    }
}