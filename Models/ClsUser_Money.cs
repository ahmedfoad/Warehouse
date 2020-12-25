using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models
{
    public class ClsUser_Money
    {


        public string ID { get; set; }
   

        public int User_Id { get; set; }
        public string User_Name { get; set; }
        public string Money_Invoice_Mandob { get; set; }
        public string Money_Sarf { get; set; }
        public string Money_Sandok { get; set; }
        public string Money_Mada { get; set; }
        public string Money_Remain { get; set; }
        public string Save_Date { get; set; }
    }
}