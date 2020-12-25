using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class ClsUser
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EMAIL { get; set; }
        public int ROLE { get; set; }
    
       
        public System.DateTime INDATE { get; set; }
        public bool STOPEMP { get; set; }
        public int Account_Type { get; set; }
    }
}