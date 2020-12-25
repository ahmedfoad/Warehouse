using Warehouse.Models;
using Warehouse.Models.Administration;
using Warehouse.Models.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Warehouse.GlobalClass;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Warehouse.CustomFilters;
using System.Globalization;

namespace Warehouse.Controllers.Main
{
    public class SearchController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 50;
        ErrorViewModel Error = new ErrorViewModel();
        #region Search Product
        int viewid_Product = 2482;// الإستعلام عن صنف
                                  // GET: Search
        [HttpGet]
        [CustomAuthorize(Roles = "الإستعلام عن صنف&enter$1")]
        public ActionResult Product()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Product(int? page, Srch_Product Srch_Product)
        {

            Barcode b = new Barcode();

            var AllRecord = db.Products.AsQueryable();

            if (Srch_Product.Company_id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Company_Id == Srch_Product.Company_id);
            }
            if (string.IsNullOrEmpty(Srch_Product.Product_Name) == false)
            {
                AllRecord = AllRecord.Where(x => x.Name.Contains(Srch_Product.Product_Name));
            }
            if (Srch_Product.Price_From != 0 || Srch_Product.Price_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Price_Unit >= Srch_Product.Price_From && x.Price_Unit <= Srch_Product.Price_To);
            }
            if (Srch_Product.Price_Mowrid_From != 0 || Srch_Product.Price_Mowrid_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Price_Mowrid >= Srch_Product.Price_Mowrid_From && x.Price_Mowrid <= Srch_Product.Price_Mowrid_To);
            }
            if (Srch_Product.Barcode != "")
            {
                AllRecord = AllRecord.Where(x => x.Barcode == Srch_Product.Barcode);
            }

            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            List<ClsProduct> RecordList = new List<ClsProduct>();
            var products = AllRecord
         .OrderBy(s => s.ID).Skip(skipRecords)
         .Take(recordsPerPage).ToList();
            foreach (var a in products)
            {
                ClsProduct ClsProduct = new ClsProduct
                {
                    ID = a.ID,
                    Name = a.Name,
                    Company_Name = a.Company.Name,

                    Price_Unit = a.Price_Unit,
                    Price_Mowrid = a.Price_Mowrid,
                    Taxes = a.Taxes,
                    Taxes_Price = Math.Round(((a.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero),
                    TotalPrice = Math.Round(((a.Price_Mowrid) + (a.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero)
                };

                Invoice_Company_Product Invoice_Company_Product = db.Invoice_Company_Product.Where(x => x.Product_Id == ClsProduct.ID).OrderByDescending(x => x.ID).FirstOrDefault();
                if (Invoice_Company_Product != null)
                {
                    ClsProduct.Price_Mowrid = Invoice_Company_Product.Price;
                    ClsProduct.TotalPrice = Math.Round(((ClsProduct.Price_Mowrid) + (ClsProduct.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                }
                RecordList.Add(ClsProduct);
            }


            if (Srch_Product.Price_From != 0 || Srch_Product.Price_To != 0)
            {
                RecordList = RecordList.Where(x => x.Price_Unit >= Srch_Product.Price_From && x.Price_Unit <= Srch_Product.Price_To).ToList();
            }
            if (Srch_Product.Price_Mowrid_From != 0 || Srch_Product.Price_Mowrid_To != 0)
            {
                RecordList = RecordList.Where(x => x.Price_Mowrid >= Srch_Product.Price_Mowrid_From && x.Price_Mowrid <= Srch_Product.Price_Mowrid_To).ToList();
            }
            var list = JsonConvert.SerializeObject(RecordList, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list, "application/json");

        }

        private void CheckUpdateBarcode(Product product, string Barcode)
        {
            decimal test = decimal.Parse(Barcode);
            string _barcode = test.ToString();


            if (product.Barcode == Barcode)
            {
                return;
            }
            else
            {
                if (product.Barcode == _barcode)
                {
                    product.Barcode = Barcode;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }


            }
        }

        #endregion

        #region Search Invoice_Mandob
        [HttpGet]
        [CustomAuthorize(Roles = "الإستعلام عن حركة بيع&enter")]
        // GET: Search
        public ActionResult Invoice_Mandob()
        {
            return View();
            
        }

        [HttpPost]
        public ActionResult Invoice_Mandob(int? page, Srch_Invoice_Mandob Srch_Mandob)
        {
            List<Cls_Invoice_Mandob> RecordList = new List<Cls_Invoice_Mandob>();
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecord = db.Invoice_Mandob.AsQueryable();
            if (Srch_Mandob.Invoice_From != 0 || Srch_Mandob.Invoice_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Invoice_Number >= Srch_Mandob.Invoice_From && x.Invoice_Number <= Srch_Mandob.Invoice_To);
            }
            if ((Srch_Mandob.DateFrom != null && Srch_Mandob.DateFrom != "" && Srch_Mandob.DateTo != null && Srch_Mandob.DateTo != ""))
            {
                DateTime Date_Invoice_From = DateTime.ParseExact(Srch_Mandob.DateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Date_Invoice_TO = DateTime.ParseExact(Srch_Mandob.DateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = Date_Invoice_TO.AddDays(1);
                AllRecord = AllRecord.Where(x => x.Date_Invoice >= Date_Invoice_From && x.Date_Invoice < endDateExclusive);
            }
            if (Srch_Mandob.Mandob_id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Mandob_id == Srch_Mandob.Mandob_id);
            }

            if (Srch_Mandob.Price_From != 0 || Srch_Mandob.Price_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Price >= Srch_Mandob.Price_From && x.Price <= Srch_Mandob.Price_To);
            }
            RecordList = AllRecord.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Invoice_Mandob
             {
                 ID = a.ID,
                 Invoice_Number = a.Invoice_Number,
                 Mandob_id = a.Mandob_id,
                 Mandob_Name = a.Mandob.Name,
                 _Date_Invoice = a.Date_Invoice,
                 Price = a.Price,
                 
                 //TotalPrice = Math.Round(((a.Price) + (a.Price * (decimal)0.05)), 2),
                 Total_Sadad = a.Total_Sadad
             }).ToList();
            var list = JsonConvert.SerializeObject(RecordList,
                        Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        });

            return Content(list, "application/json");

        }
        #endregion

        #region Search Invoice_Company
        // GET: Search
        [CustomAuthorize(Roles = "الإستعلام عن عملية شراء&enter")]
        public ActionResult Invoice_Company()
        {
            return View();
            
        }

        [HttpPost]
        public ActionResult Invoice_Company(int? page, Srch_Invoice_Company Srch_Company)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            List<Cls_Invoice_Company> RecordList = new List<Cls_Invoice_Company>();
            if (db.Invoice_Company.Any())
            {
                var AllRecord = db.Invoice_Company.AsQueryable();
                if (Srch_Company.Invoice_From != 0 || Srch_Company.Invoice_To != 0)
                {
                    AllRecord = AllRecord.Where(x => x.Invoice_Number >= Srch_Company.Invoice_From && x.Invoice_Number <= Srch_Company.Invoice_To);
                }
                if ((Srch_Company.DateFrom != null && Srch_Company.DateFrom != "" && Srch_Company.DateTo != null && Srch_Company.DateTo != ""))
                {
                    DateTime Date_Invoice_From = DateTime.ParseExact(Srch_Company.DateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime Date_Invoice_TO = DateTime.ParseExact(Srch_Company.DateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var endDateExclusive = Date_Invoice_TO.AddDays(1);
                    AllRecord = AllRecord.Where(x => x.Date_Invoice >= Date_Invoice_From && x.Date_Invoice < endDateExclusive);
                }
                if (Srch_Company.Company_id != 0)
                {
                    AllRecord = AllRecord.Where(x => x.Company_id == Srch_Company.Company_id);
                }

                if (Srch_Company.Price_From != 0 || Srch_Company.Price_To != 0)
                {
                    AllRecord = AllRecord.Where(x => x.Price >= Srch_Company.Price_From && x.Price <= Srch_Company.Price_To);
                }

                //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))

                RecordList = AllRecord.OrderBy(s => s.ID).Skip(skipRecords)
                 .Take(recordsPerPage).Select(a => new Cls_Invoice_Company
                 {
                     ID = a.ID,
                     Invoice_Number = a.Invoice_Number,
                     Company_id = a.Company_id,
                     Company_Name = a.Company.Name,
                     _Date_Invoice = a.Date_Invoice,
                     Price = a.Price,
                     Taxes = a.Taxes,
                     Total_Sadad = a.Total_Sadad
                 }).ToList();
            }
            var list = JsonConvert.SerializeObject(RecordList,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }
        #endregion

        #region Search  Mandob
        // GET: Search
        
        public ActionResult Mandob()
        {
            return View();
            
        }

        [HttpPost]
        public ActionResult Mandob(int? page, Srch_Mandob Srch_Mandob)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecords = db.Mandobs
             //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
             .OrderBy(s => s.Name).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Srch_Mandob
             {
                 ID = a.ID,
                 Name = a.Name,
                 //Sejil = a.Sejil ?? default(int),
                 //JawalNO = a.JawalNO,
                 //Address = a.Address
             }).ToList();
            var list = JsonConvert.SerializeObject(AllRecords,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }
        #endregion


        #region Search  company
        // GET: Search
        [CustomAuthorize(Roles = "الإستعلام عن شركة&enter")]
        public ActionResult Company()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Company(int? page, Srch_Company Srch_Company)
        {

            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecords = db.Companies
             //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
             .OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Srch_Company
             {
                 ID = a.ID,
                 Name = a.Name,

             }).ToList();
            var list = JsonConvert.SerializeObject(AllRecords,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");

        }
        #endregion


        #region Search Masrofat
        [CustomAuthorize(Roles = "الإستعلام عن المصروفات&enter")]
        // GET: Search
        public ActionResult Masrofat()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Masrofat(int? page, Srch_Masrofat RModel)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecord = db.Masrofats.AsQueryable();
            if (RModel.ID_From != 0 || RModel.ID_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Invoice_Number >= RModel.ID_From && x.Invoice_Number <= RModel.ID_To);
            }
            if (RModel.Masrofat_Type_Id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Masrofat_Type_Id == RModel.Masrofat_Type_Id);
            }
            if (string.IsNullOrEmpty(RModel.Bian) == false)
            {
                AllRecord = AllRecord.Where(x => x.Bian.Contains(RModel.Bian));
            }
            if ((RModel.Date_Invoice_From != null && RModel.Date_Invoice_From != "" && RModel.Date_Invoice_TO != null && RModel.Date_Invoice_TO != ""))
            {
                DateTime Date_Invoice_From = DateTime.ParseExact(RModel.Date_Invoice_From, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Date_Invoice_TO = DateTime.ParseExact(RModel.Date_Invoice_TO, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = Date_Invoice_TO.AddDays(1);
                AllRecord = AllRecord.Where(x => x.Date_Invoice >= Date_Invoice_From && x.Date_Invoice < endDateExclusive);
            }

            if (RModel.Money_From != 0 || RModel.Money_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Money >= RModel.Money_From && x.Money <= RModel.Money_To);
            }
            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
            var RecordList = AllRecord.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Masrofat
             {
                 ID = a.ID,
                 Invoice_Number = a.Invoice_Number,
                 Masrofat_Type_Id = a.Masrofat_Type_Id,
                 Masrofat_Type_Name = a.Masrofat_Type.Name,
                 _Date_Invoice = a.Date_Invoice,
                 Money = a.Money,
                 Bian = a.Bian,
                 IsEdit = (a.Invoice_Company_Sadad_ID == null && a.Invoice_Mandob_ID == null && a.Salary_ID == null) ? 1: 0
             }).ToList();
            var list = JsonConvert.SerializeObject(RecordList,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }
        #endregion

        #region Search Eradat
        [CustomAuthorize(Roles = "الإستعلام عن الايرادات&enter")]
        // GET: Search
        public ActionResult Eradat()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Eradat(int? page, Srch_Eradat RModel)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecord = db.Eradats.AsQueryable();
            if (RModel.ID_From != 0 || RModel.ID_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Invoice_Number >= RModel.ID_From && x.Invoice_Number <= RModel.ID_To);
            }
            if (RModel.Eradat_Type_Id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Eradat_Type_Id == RModel.Eradat_Type_Id);
            }
            if (string.IsNullOrEmpty(RModel.Bian) == false)
            {
                AllRecord = AllRecord.Where(x => x.Bian.Contains(RModel.Bian));
            }
            if ((RModel.Date_Invoice_From != null && RModel.Date_Invoice_From != "" && RModel.Date_Invoice_TO != null && RModel.Date_Invoice_TO != ""))
            {
                DateTime Date_Invoice_From = DateTime.ParseExact(RModel.Date_Invoice_From, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Date_Invoice_TO = DateTime.ParseExact(RModel.Date_Invoice_TO, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = Date_Invoice_TO.AddDays(1);
                AllRecord = AllRecord.Where(x => x.Date_Invoice >= Date_Invoice_From && x.Date_Invoice < endDateExclusive);
            }

            if (RModel.Money_From != 0 || RModel.Money_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Money >= RModel.Money_From && x.Money <= RModel.Money_To);
            }
            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
            var RecordList = AllRecord.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Eradat
             {
                 ID = a.ID,
                 Invoice_Number = a.Invoice_Number,
                 Eradat_Type_Id = a.Eradat_Type_Id,
                 Eradat_Type_Name = a.Eradat_Type.Name,
                 _Date_Invoice = a.Date_Invoice,
                 Money = a.Money,
                 Bian = a.Bian,
                 IsEdit = (a.Invoice_Mandob_Sadad_ID == null) ? 1 : 0
             }).ToList();
            var list = JsonConvert.SerializeObject(RecordList,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }
        #endregion

        #region Search Employee
        [CustomAuthorize(Roles = "الإستعلام عن الموظفين&enter")]
        // GET: Search
        public ActionResult Employee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Employee(int? page, Srch_Employee RModel)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecord = db.Employees.AsQueryable();
            if (string.IsNullOrEmpty(RModel.Name) == false)
            {
                AllRecord = AllRecord.Where(x => x.Name.Contains(RModel.Name));
            }
            if (string.IsNullOrEmpty(RModel.Mobile) == false)
            {
                AllRecord = AllRecord.Where(x => x.Mobile.Contains(RModel.Mobile));
            }
            if (RModel.Job_Id != 0)
            {
                AllRecord = AllRecord.Where(x => x.Job_Id == RModel.Job_Id);
            }
            if (RModel.Salary_From != 0 || RModel.Salary_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Salary >= RModel.Salary_From && x.Salary <= RModel.Salary_To);
            }

            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
            var RecordList = AllRecord.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Employee
             {
                 ID = a.ID,
                 Name = a.Name,
                 Salary = a.Salary,
                 Mobile = a.Mobile,
                 Job_Name = a.Job.Name
             }).ToList();
            var list = JsonConvert.SerializeObject(RecordList,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }
        #endregion


        #region Search Salary
        [CustomAuthorize(Roles = "الإستعلام عن الرواتب&enter")]
        // GET: Search
        public ActionResult Salary()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Salary(int? page, Srch_Salary RModel)
        {
            var skipRecords = ((page != null && page >= 1) ? page.Value : 0) * recordsPerPage;
            var AllRecord = db.Salaries.AsQueryable();
            if (RModel.ID_From != 0 || RModel.ID_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.Invoice_Number >= RModel.ID_From && x.Invoice_Number <= RModel.ID_To);
            }
            
            if (string.IsNullOrEmpty(RModel.Bian) == false)
            {
                AllRecord = AllRecord.Where(x => x.Notes.Contains(RModel.Bian));
            }
            if ((RModel.Date_Invoice_From != null && RModel.Date_Invoice_From != "" && RModel.Date_Invoice_TO != null && RModel.Date_Invoice_TO != ""))
            {
                DateTime Date_Invoice_From = DateTime.ParseExact(RModel.Date_Invoice_From, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime Date_Invoice_TO = DateTime.ParseExact(RModel.Date_Invoice_TO, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var endDateExclusive = Date_Invoice_TO.AddDays(1);
                AllRecord = AllRecord.Where(x => x.DateFrom >= Date_Invoice_From && x.DateFrom < endDateExclusive);
                AllRecord = AllRecord.Where(x => x.DateTO >= Date_Invoice_From && x.DateTO < endDateExclusive);
            }

            if (RModel.Money_From != 0 || RModel.Money_To != 0)
            {
                AllRecord = AllRecord.Where(x => x.TotalSalaries >= RModel.Money_From && x.TotalSalaries <= RModel.Money_To);
            }
            //.Where(s => string.IsNullOrEmpty(UserName) ? true : s.Name.Contains(UserName))
            var RecordList = AllRecord.OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).Select(a => new Cls_Salary
             {
                 ID = a.ID,
                 Invoice_Number = a.Invoice_Number,
                 _DateFrom = a.DateFrom,
                 _DateTO = a.DateTO,
                 TotalSalaries = a.TotalSalaries,
                 Notes = a.Notes
             }).ToList();
            var list = JsonConvert.SerializeObject(RecordList,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }


}
