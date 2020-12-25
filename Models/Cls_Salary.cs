using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class Cls_Salary
    {
        

        public int ID { get; set; }
        public int Invoice_Number { get; set; }
        public string DateFrom { get; set; }
        public string DateTO { get; set; }

        public DateTime _DateFrom { get; set; }
        public DateTime _DateTO { get; set; }

        public string Notes { get; set; }
        public decimal TotalSalaries { get;  set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public string InDate { get; set; }
        public List<Cls_Employee_Salary> Cls_Employee_Salary { get; set; }
        public string User_Name { get;  set; }
    }
    public class Cls_Employee_Salary {
        public int ID { get;  set; }
        public string Name { get; set; }
        public string Job_Name { get; set; }
        public string Mobile { get; set; }
        public int Salary_Id { get; set; }
        public int Employee_Id { get; set; }
        public decimal Salary { get; set; }

    }
}