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
    
    public partial class Salary
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Salary()
        {
            this.Employee_Salary = new HashSet<Employee_Salary>();
            this.Masrofats = new HashSet<Masrofat>();
        }
    
        public int ID { get; set; }
        public int Invoice_Number { get; set; }
        public System.DateTime DateFrom { get; set; }
        public System.DateTime DateTO { get; set; }
        public string Notes { get; set; }
        public decimal TotalSalaries { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public System.DateTime InDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public int Userid_In { get; set; }
        public Nullable<int> userid_Edit { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee_Salary> Employee_Salary { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Masrofat> Masrofats { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}