using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class Cls_Company_SadadList
    {
        public int Company_id { get; set; }
        public string Company_Name { get; set; }
        public decimal Price { get; set; }
        public decimal Total_Sadad { get; set; }
        // public decimal Total_Sadad_Orignal { get; set; }
        public decimal Money { get; set; }
        public int Sadad_Type_Id { get; set; }
    }
}