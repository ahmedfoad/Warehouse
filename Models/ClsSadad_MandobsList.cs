﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class ClsSadad_MandobsList
    {
        public int Mandob_id { get; set; }
        public string Mandob_Name { get; set; }
        public decimal Price { get; set; }
        public decimal Total_Sadad { get; set; }
        public decimal Remain { get; set; }
        public object Omola { get;  set; }

        //public int Sadad_Type_Id { get; set; }
    }
}