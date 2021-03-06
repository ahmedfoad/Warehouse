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
    
    public partial class Invoice_Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice_Company()
        {
            this.Invoice_Company_Product = new HashSet<Invoice_Company_Product>();
            this.Invoice_Company_Sadad = new HashSet<Invoice_Company_Sadad>();
        }
    
        public int ID { get; set; }
        public int Invoice_Number { get; set; }
        public int Company_id { get; set; }
        public System.DateTime Date_Invoice { get; set; }
        public string Date_Invoice_Hijri { get; set; }
        public decimal Nakl_Cost { get; set; }
        public decimal Price { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total_Sadad { get; set; }
        public int User_ID { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public System.DateTime InDate { get; set; }
        public bool Is_Deleted { get; set; }
        public bool IS_Chekced { get; set; }
    
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Company_Product> Invoice_Company_Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Company_Sadad> Invoice_Company_Sadad { get; set; }
        public virtual User User { get; set; }
    }
}
