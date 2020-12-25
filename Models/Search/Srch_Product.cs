using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Models.Search
{
       
public class Srch_Product
{
    public string Product_Name { get; set; }
    public int Company_id { get; set; }
    public string Company_Name { get; set; }
    public decimal Price_From { get; set; }
    public decimal Price_To { get; set; }
    public decimal Price_Mowrid_From { get; set; }
    public decimal Price_Mowrid_To { get; set; }
    public string Barcode { get; set; }
}
}