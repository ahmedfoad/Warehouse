using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;

namespace Warehouse.Models
{
    public class View_Roles
    {
        public _View _View { get; set; }
        public bool Role_Enter { get; set; }
        public bool Role_Save { get; set; }
        public bool Role_Edit { get; set; }
        public bool Role_Delete { get; set; }

    }
    public class _View
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}