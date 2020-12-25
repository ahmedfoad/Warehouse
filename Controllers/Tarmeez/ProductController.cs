using Warehouse.Models;
using Warehouse.Models.Administration;
using Warehouse.Models.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Warehouse.GlobalClass;
using Warehouse.GlobalClass.Reports;


using Warehouse.Models.Reports;
using DevExpress.XtraPrinting;
using Warehouse.Reports.Product;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Warehouse.CustomFilters;

namespace Warehouse.Controllers.Main
{
    public class ProductController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        ErrorViewModel Error = new ErrorViewModel();

        private int recordsPerPage = 300;
        int viewid = 2470;// الصنف
        /*
        [HttpGet]
        public ActionResult GetMaxBarcode()
        {
            decimal barcode = 1;
            if (db.Products.Any())
            {
                barcode = (db.Products.MaxAsync(a => a.Barcode)) + 1;
            }
            var jsonResult = Json(barcode, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }*/

        [HttpGet]
        public ActionResult GetCompanies()
        {
            return PartialView();
        }
        public ActionResult getAllProducts(int? page, string Search, int Customer_Type = 1)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            User user = db.Users.Find(UserID);
            int Account_Type = user.Account_Type;

            // decimal dee = Math.Round((((decimal)9.5 * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;
            List<ClsProduct> RecordList = new List<ClsProduct>();
            var products = db.Products.AsQueryable();
            products = products.Where(s => string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search))
            .OrderBy(s => s.Name).Skip(skipRecords != null ? skipRecords.Value : 0).Take(recordsPerPage);

            if (Customer_Type == 1)//مندوب
            {
                products = products.Where(a => a.Price_Unit > 0);
            }
            else if (Customer_Type == 2)//محل
            {
                products = products.Where(a => a.Shop_Price > 0);
            }
            else if (Customer_Type == 3)//منزل
            {
                products = products.Where(a => a.Home_Price > 0);
            }

            List<Product> AllRecords = products.ToList();
            foreach (var item in AllRecords)
            {
                ClsProduct ClsProduct = new ClsProduct();
                ClsProduct.ID = item.ID;
                ClsProduct.Name = item.Name;
                ClsProduct.Company_Name = item.Company.Name;
                if (Customer_Type == 1)//مندوب
                {
                    ClsProduct.Price_Unit = item.Price_Unit;

                    ClsProduct.Offer_TargetAmount = item.Offer_TargetAmount;
                    ClsProduct.Offer_BonusAmount = item.Offer_BonusAmount;
                    ClsProduct.Offer_Product_id = item.Offer_Product_id;
                    ClsProduct.Offer_Product_Name = (item.Offer_Product_id != null) ? item.Product1.Name : "";
                }
                else if (Customer_Type == 2)//محل
                {
                    ClsProduct.Price_Unit = item.Shop_Price ?? default(decimal);

                    ClsProduct.Offer_TargetAmount = item.Shop_Offer_TargetAmount;
                    ClsProduct.Offer_BonusAmount = item.Shop_Offer_BonusAmount;
                    ClsProduct.Offer_Product_id = item.Shop_Offer_Product_id;
                    ClsProduct.Offer_Product_Name = (item.Shop_Offer_Product_id != null) ? item.Product2.Name : "";
                }
                else if (Customer_Type == 3)//منزل
                {
                    ClsProduct.Price_Unit = item.Home_Price ?? default(decimal);

                    ClsProduct.Offer_TargetAmount = item.Home_TargetAmount;
                    ClsProduct.Offer_BonusAmount = item.Home_Offer_BonusAmount;
                    ClsProduct.Offer_Product_id = item.Home_Offer_Product_id;
                    ClsProduct.Offer_Product_Name = (item.Home_Offer_Product_id != null) ? item.Product3.Name : "";
                }
               
                ClsProduct.Price_Mowrid = item.Price_Mowrid;
                if (Account_Type == 1) // admin
                {
                    ClsProduct.Price_Mowrid_Copy = item.Price_Mowrid;
                }

                ClsProduct.Taxes = item.Taxes;
                ClsProduct.Taxes_Price = Math.Round(((item.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                ClsProduct.Barcode = item.Barcode;
                ClsProduct.TotalPrice = (item.Price_Mowrid) + Math.Round(((item.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                //*****************************************************************************************************************
                ////////////////Invoice_Company_Product Invoice_Company_Product = db.Invoice_Company_Product.Where(a => a.Product_Id == item.ID).OrderByDescending(a => a.ID).FirstOrDefault();
                ////////////////if (Invoice_Company_Product != null)
                ////////////////{
                ////////////////    ClsProduct.Price_Mowrid = Invoice_Company_Product.Price;
                ////////////////    ClsProduct.TotalPrice = (ClsProduct.Price_Mowrid) + (ClsProduct.Price_Mowrid * (decimal)0.05);
                ////////////////    ClsProduct.Taxes_Price = Math.Round(((ClsProduct.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                ////////////////}
                //*****************************************************************************************************************
                RecordList.Add(ClsProduct);
            }



            var list = JsonConvert.SerializeObject(RecordList, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
            //return Json(AllRecords, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult Insert(ClsProduct ClsProduct)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            if (db.Products.Where(a =>
            ClsProduct.Barcode != null
            && ClsProduct.Barcode != null
            && a.Barcode == ClsProduct.Barcode).Any() == false)
            {
                WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
                List<string> computerDetails = identity.Name.Split('\\').ToList();




                Product product = new Product
                {
                    Barcode = ClsProduct.Barcode,
                    Company_Id = ClsProduct.Company_Id,
                    Name = ClsProduct.Name,
                    Price_Unit = ClsProduct.Price_Unit,
                    Price_Mowrid = ClsProduct.Price_Mowrid,
                    Taxes = ClsProduct.Taxes,
                    OldAmount = ClsProduct.OldAmount,
                    Offer_Product_id = ClsProduct.Offer_Product_id,
                    Offer_TargetAmount = ClsProduct.Offer_TargetAmount,
                    Offer_BonusAmount = ClsProduct.Offer_BonusAmount,


                    Home_Price = ClsProduct.Home_Price,
                    Home_Offer_Product_id = ClsProduct.Home_Offer_Product_id,
                    Home_TargetAmount = ClsProduct.Home_TargetAmount,
                    Home_Offer_BonusAmount = ClsProduct.Home_Offer_BonusAmount,

                    Shop_Price = ClsProduct.Shop_Price,
                    Shop_Offer_Product_id = ClsProduct.Shop_Offer_Product_id,
                    Shop_Offer_TargetAmount = ClsProduct.Shop_Offer_TargetAmount,
                    Shop_Offer_BonusAmount = ClsProduct.Shop_Offer_BonusAmount,

                    User_ID = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = DateTime.Now
                };
                db.Products.Add(product);
                db.SaveChanges();

                //UserAction UserAction = new UserAction
                //{
                //    Userid = UserID,
                //    Viewid = viewid,
                //    ActionDate = DateTime.Now,
                //    Action = ((_Action)1).ToString(),//حفظ
                //    Operation = "حفظ صنف جديد اسم الصنف : " + product.Name + "-"
                //          + " ورقم الباركود  : " + product.Barcode.ToString()
                //};
                //db.UserActions.Add(UserAction);
                //db.SaveChanges();
                Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
                Error.ID = product.ID;
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Error.ErrorFullNumber = "AR-Insert-081";
                Error.ErrorNumber = "081";
                Error.Url = "/Home";
                Error.ErrorName = "خطأ الاسم موجود مسبقا";
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult InsertProductList(List<ClsProduct> model)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            foreach (var item in model)
            {
                int index = model.IndexOf(item);
                int count = model.Count;

                //System.Globalization.DateTimeFormatInfo HijriDTFI;
                //HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
                //HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
                //HijriDTFI.ShortDatePattern = "dd/MM/yyyy";
                //DateTime Date_Expiration = DateTime.ParseExact(item.Date_Expiration, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //string Date_Expiration_Hijri = Date_Expiration.Date.ToString("dd/MM/yyyy", HijriDTFI);


                WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
                List<string> computerDetails = identity.Name.Split('\\').ToList();

                Product Product = new Product
                {
                    User_ID = UserID,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    InDate = DateTime.Now,
                    Company_Id = item.Company_Id,
                    Barcode = item.Barcode,

                    Price_Unit = item.Price_Unit,
                    Price_Mowrid = item.Price_Mowrid,
                    Taxes = item.Taxes,
                    Name = item.Name
                };

                db.Products.Add(Product);


                //UserAction UserAction = new UserAction
                //{
                //    Userid = UserID,
                //    Viewid = viewid,
                //    ActionDate = DateTime.Now,
                //    Action = ((_Action)1).ToString(),//حفظ
                //    Operation = "حفظ صنف جديد اسم الصنف : " + item.Name + "-"
                //          + " ورقم الباركود  : " + item.Barcode.ToString()
                //};
                //db.UserActions.Add(UserAction);

            }
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult loadProduct(int id)
        {
            Cls_Product Cls_Product = new Cls_Product();
            Product Product = db.Products.Find(id);
            if (Product != null)
            {

                Cls_Product.ClsProduct = new ClsProduct
                {
                    ID = Product.ID,
                    Barcode = Product.Barcode,
                    Company_Id = Product.Company_Id,
                    ComputerName = Product.ComputerName,
                    ComputerUser = Product.ComputerUser,
                    Company_Name = Product.Company.Name,
                    Name = Product.Name,
                    InDate = Product.InDate,
                    Price_Unit = Product.Price_Unit,
                    Price_Mowrid = Product.Price_Mowrid,
                    Taxes = Product.Taxes,
                    OldAmount = Product.OldAmount,
                    User_Name = (Product.User != null) ? Product.User.NAME : "",
                    Offer_TargetAmount = Product.Offer_TargetAmount,
                    Offer_BonusAmount = Product.Offer_BonusAmount,
                    Offer_Product_id = Product.Offer_Product_id,
                    Offer_Product_Name = (Product.Product1 != null) ? Product.Product1.Name : "",

                    Shop_Offer_TargetAmount = Product.Shop_Offer_TargetAmount,
                    Shop_Offer_BonusAmount = Product.Offer_BonusAmount,
                    Shop_Offer_Product_id = Product.Offer_Product_id,
                    Shop_Offer_Product_Name = (Product.Product2 != null) ? Product.Product2.Name : "",
                    Shop_Price = Product.Shop_Price,

                    Home_TargetAmount = Product.Home_TargetAmount,
                    Home_Offer_BonusAmount = Product.Offer_BonusAmount,
                    Home_Offer_Product_id = Product.Offer_Product_id,
                    Home_Offer_Product_Name = (Product.Product3 != null) ? Product.Product3.Name : "",
                    Home_Price = Product.Home_Price
                };

                Barcode Barcode = new Barcode();
                Image BarCodeImage = Barcode.getBarCode(Convert.ToInt64(Cls_Product.ClsProduct.Barcode).ToString("D12"), 3500, 2000);
                Cls_Product.BarCodeArr = Barcode.imageToByteArray(BarCodeImage);
                Cls_Product.ErrorName = "تم تحميل بيانات المعاملة الصادرة بنجاح";
            }
            var list = JsonConvert.SerializeObject(Cls_Product,
           Formatting.None,
           new JsonSerializerSettings()
           {
               ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
           });
            return Content(list, "application/json");
        }
        [HttpPost]
        public ActionResult Edit(ClsProduct model)
        {


            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            Product item = db.Products.Find(model.ID);
            item.Barcode = model.Barcode;
            item.Company_Id = model.Company_Id;
            //item.ComputerUser = Product.ComputerUser,
            //item.Company_Name = Product.Company.Name,
            item.Barcode = model.Barcode;
            item.Company_Id = model.Company_Id;
            item.ComputerName = model.ComputerName;
            item.ComputerUser = model.ComputerUser;

            item.Name = model.Name;
            item.OldAmount = model.OldAmount;

            item.Price_Unit = model.Price_Unit;
            item.Price_Mowrid = model.Price_Mowrid;
            item.Taxes = model.Taxes;
            item.User_ID = UserID;
            item.Offer_Product_id = model.Offer_Product_id;
            item.Offer_TargetAmount = model.Offer_TargetAmount;
            item.Offer_BonusAmount = model.Offer_BonusAmount;

            item.Home_Price = model.Home_Price;
            item.Home_Offer_Product_id = model.Home_Offer_Product_id;
            item.Home_TargetAmount = model.Home_TargetAmount;
            item.Home_Offer_BonusAmount = model.Home_Offer_BonusAmount;

            item.Shop_Price = model.Shop_Price;
            item.Shop_Offer_Product_id = model.Shop_Offer_Product_id;
            item.Shop_Offer_TargetAmount = model.Shop_Offer_TargetAmount;
            item.Shop_Offer_BonusAmount = model.Shop_Offer_BonusAmount;


            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = item.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize(Roles = "الصنف&save$1")]
        public ActionResult Operation()
        {
            return View();
        }
        //جلب الامنتجات لفاتورة العميل----------------------
        [HttpGet]
        public ActionResult GetProducts()
        {
            return PartialView();
        }
        //البحث عن صنف العميل----------------------
        [HttpGet]
        public ActionResult GetSearchProducts()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetbyBarcode(string BarCode)
        {
            var a = db.Products.Where(s => s.Barcode == BarCode).FirstOrDefault();
            if (a != null)
            {

                var AllRecords = new ClsProduct
                {
                    ID = a.ID,
                    Name = a.Name,
                    Barcode = a.Barcode,
                    Company_Id = a.Company_Id,
                    Price_Unit = a.Price_Unit,
                    Taxes = a.Taxes

                };
                var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
                return Content(list, "application/json");
            }
            else
            {
                decimal _barcode = decimal.Parse(BarCode);
                a = db.Products.Where(s => s.Barcode == _barcode.ToString()).FirstOrDefault();
                if (a != null)
                {

                    var AllRecords = new ClsProduct
                    {
                        ID = a.ID,
                        Name = a.Name,
                        Barcode = a.Barcode,
                        Company_Id = a.Company_Id,
                        Price_Unit = a.Price_Unit,
                        Taxes = a.Taxes

                    };
                    CheckUpdateBarcode(a, BarCode);
                    var list = JsonConvert.SerializeObject(AllRecords, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
                    return Content(list, "application/json");
                }
            }


            var list2 = JsonConvert.SerializeObject(null, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Content(list2, "application/json");
        }




        [HttpGet]
        public ActionResult GetbyBarcode_ForOrders(string BarCode)
        {

            Barcode b = new Barcode();

            string _mycode;


            Product product = db.Products
            .Where(a =>


               (a.Barcode == BarCode)

     ).FirstOrDefault();
            if (product != null)
            {

                decimal Taxes_new = Math.Round(((product.Price_Unit * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                var AllRecords = new ClsProduct
                {
                    ID = product.ID,
                    Name = product.Name,
                    Barcode = product.Barcode,
                    Company_Id = product.Company_Id,
                    Price_Unit = product.Price_Unit,
                    Taxes = Taxes_new,
                };
                var list = JsonConvert.SerializeObject(AllRecords,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });

                return Content(list, "application/json");
            }
            else
            {
                decimal test = 0;
                if (BarCode != null)
                {
                    decimal.TryParse(BarCode, out test);
                }
                string _barcode = test.ToString();
                product = db.Products
            .Where(a =>
                    (a.Barcode == _barcode)


     ).FirstOrDefault();
                if (product != null)
                {
                    CheckUpdateBarcode(product, BarCode);
                    decimal Taxes_new = Math.Round(((product.Price_Unit * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
                    var AllRecords = new ClsProduct
                    {
                        ID = product.ID,
                        Name = product.Name,
                        Barcode = product.Barcode,
                        Company_Id = product.Company_Id,
                        Price_Unit = product.Price_Unit,
                        Taxes = Taxes_new,
                    };
                    var list = JsonConvert.SerializeObject(AllRecords,
                    Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    });

                    return Content(list, "application/json");
                }
            }

            return null;
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

        public ActionResult ProductList()
        {
            return View();
        }





        //*************************************************************************
        // طباعة باركود الصنف و الكرتون و الصندوق
        //*************************************************************************
        //----------------------------------------
        [HttpGet]
        public ActionResult PrintBarCodeReport(decimal id)
        {

            ViewBag.MyUrl = "/product/PDF_ProductBarcode?id=" + id.ToString();
            return PartialView("Print_BarcodeProduct");

        }
        [HttpGet]
        public ActionResult PDF_ProductBarcode(int id)
        {
            List<Prnt_ProductBarcode> list = new List<Prnt_ProductBarcode>();
            Product Product = db.Products.Find(id);
            Barcode Barcode = new Barcode();
            Image BarCodeImage = Barcode.getBarCode(Convert.ToInt64(Product.Barcode).ToString("D12"), 1750, 1000);
            decimal Taxes_new = Math.Round(((Product.Price_Mowrid * (decimal)0.05)), 3, MidpointRounding.AwayFromZero);
            decimal Price_new = Math.Round((Product.Price_Mowrid + Taxes_new), 2, MidpointRounding.AwayFromZero);
            Prnt_ProductBarcode ClsProduct = new Prnt_ProductBarcode
            {
                Barcode = Product.Barcode,
                Company_Id = Product.Company_Id,
                ComputerName = Product.ComputerName,
                ComputerUser = Product.ComputerUser,
                Company_Name = Product.Company.Name,
                Name = ConvertToEasternArabicNumerals.ConvertAR(Product.Name),


                InDate = Product.InDate,
                Price_Unit = ConvertToEasternArabicNumerals.ConvertAR(Price_new.ToString()),
                //,
                //Taxes = Product.Taxes,
                User_Name = (Product.User != null) ? Product.User.NAME : "",
                BarCodeImg = BarCodeImage,
                BarCodeArr = Barcode.imageToByteArray(BarCodeImage)
            };
            list.Add(ClsProduct);
            // Rpt_Product Rpt_Product = new Rpt_Product();

            //string _path = System.Web.HttpContext.Current.Server.MapPath(@"~/Reports/pdf");
            //Random random = new Random();
            //string tick = DateTime.Now.Ticks.ToString();
            //string reportPath = Path.Combine(_path, "BarCodeReport.pdf");

            BarCodeReport report = new BarCodeReport();
            report.DataSource = list;

            PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
            pdfOptions.Compressed = true;
            pdfOptions.ImageQuality = PdfJpegImageQuality.Low;
            pdfOptions.NeverEmbeddedFonts = "Tahoma;Courier New";
            pdfOptions.DocumentOptions.Application = "Human Resources Application";
            pdfOptions.DocumentOptions.Author = "مؤسسة الجود لتقنية المعلومات";
            pdfOptions.DocumentOptions.Subject = "باركود الصنف";
            pdfOptions.DocumentOptions.Title = "باركود الصنف";


            MemoryStream stream = new MemoryStream();
            report.CreateDocument();
            report.ExportToPdf(stream);
            return File(stream.GetBuffer(), "application/pdf");
        }
        //----------------------------------------

        //----------------------------------------
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