//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Warehouse.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Eradat
    {
        public int ID { get; set; }
        public int Invoice_Number { get; set; }
        public System.DateTime Date_Invoice { get; set; }
        public int Eradat_Type_Id { get; set; }
        public decimal Money { get; set; }
        public string Bian { get; set; }
        public string Notes { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public System.DateTime InDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public int Userid_In { get; set; }
        public Nullable<int> userid_Edit { get; set; }
        public Nullable<int> Invoice_Mandob_Sadad_ID { get; set; }
        public bool IS_Chekced { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
    
        public virtual Eradat_Type Eradat_Type { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual Invoice_Mandob_Sadad Invoice_Mandob_Sadad { get; set; }
    }
}
