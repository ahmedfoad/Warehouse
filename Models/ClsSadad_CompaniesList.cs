using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class ClsSadad_CompaniesList
    {
        public int Company_id { get; set; }
        public string Company_Name { get; set; }
        public decimal Price { get; set; }
        public decimal Total_Sadad { get; set; }
        public decimal Remain { get; set; }
    }
}