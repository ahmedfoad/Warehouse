using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;

namespace Warehouse.Models
{
    public class Cls_Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Job_Id { get; set; }
        public string Job_Name { get; set; }
        public decimal Salary { get; set; }
        public string Mobile { get; set; }
        public string ComputerName { get; set; }
        public string ComputerUser { get; set; }
        public string InDate { get;  set; }
        
        public string User_Name { get;  set; }
    }
}