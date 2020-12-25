namespace Warehouse.Models
{
    public class Srch_Eradat
    {
        public int ID_From { get; set; }
        public int ID_To { get; set; }
        public string Date_Invoice_From { get; set; }
        public string Date_Invoice_TO { get; set; }
        public int Eradat_Type_Id { get; set; }
        public string Eradat_Type_Name { get; set; }
        public decimal Money_From { get; set; }
        public decimal Money_To { get; set; }
        public string Bian { get; set; }
    }
}