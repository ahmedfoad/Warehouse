using Warehouse.Models;
using Warehouse.Models.Administration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Warehouse.CustomFilters;
using Warehouse.GlobalClass;

namespace Warehouse.Controllers.Tarmeez
{
    public class EmployeeController : Controller
    {
        DB_StoreEntities db = new DB_StoreEntities();
        private int recordsPerPage = 20;
        ErrorViewModel Error = new ErrorViewModel();
        ConvertToEasternArabicNumerals ConvertToEasternArabicNumerals = new ConvertToEasternArabicNumerals();
        private int viewid_Product = 1;//إضافة مستخدم جديد
        [CustomAuthorize(Roles = "اضافة موظف جديد&edit")]
        public ActionResult Operation()
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            UserView UserView = db.UserViews.Where(a => a.UserID == UserID && a.View.ID == viewid_Product).FirstOrDefault(); 

            if (UserView != null && UserView.Role_Enter == true)
            {
                return View();
            }
            else
            {
                Error.ErrorFullNumber = "AR-Open-000";
                Error.ErrorNumber = "000";
                Error.Url = "/Home";
                Error.ErrorName = "ليس لديك صلاحية الدخول إلى الإستعلام عن المعاملات الصادرة";
                return View("~/Views/Shared/ErrorPage.cshtml", Error);
            }
             
        }

        [HttpGet]
        public ActionResult Getjobs()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetEmployees()
        {
            return PartialView();
        }

        public ActionResult getAll(int? page, string Search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;
            Cls_Salary Cls_Salary = new Cls_Salary();
            Cls_Salary.Cls_Employee_Salary = new List<Cls_Employee_Salary>();
            Cls_Salary.Cls_Employee_Salary = db.Employees.Select(a => new Cls_Employee_Salary
            {
                ID = 0,
                Employee_Id = a.ID,
                Name = a.Name,
                Salary = a.Salary,
                Job_Name = a.Job.Name,
                Mobile = a.Mobile
            })
              .Where(s => string.IsNullOrEmpty(Search) ? true : s.Name.Contains(Search))
              .OrderBy(s => s.ID).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).ToList();
            var list = JsonConvert.SerializeObject(Cls_Salary, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }
        [HttpPost]
        public ActionResult Insert(Cls_Employee Cls_Employee)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            UserView UserView = db.UserViews.Where(a => a.UserID == UserID && a.View.ID == viewid_Product).FirstOrDefault();

            if (UserView != null && UserView.Role_Save == true)
            {
                WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
                List<string> computerDetails = identity.Name.Split('\\').ToList();

                Employee Employee = new Employee
                {
                    Name = Cls_Employee.Name,
                    Job_Id = Cls_Employee.Job_Id,
                    Salary = Cls_Employee.Salary,
                    Mobile = Cls_Employee.Mobile,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1],
                    Userid_In = UserID,
                    InDate = DateTime.Now
                };
                db.Employees.Add(Employee);
                db.SaveChanges();

                Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
                Error.ID = Employee.ID;
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Error.ErrorFullNumber = "AR-Open-000";
                Error.ErrorNumber = "000";
                Error.Url = "/Home";
                Error.ErrorName = "ليس لديك صلاحية الدخول إلى الإستعلام عن المعاملات الصادرة";
                return View("~/Views/Shared/ErrorPage.cshtml", Error);
            }
           
           
        }
        [HttpPost]
        public ActionResult Edit(Cls_Employee Cls_Employee)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            UserView UserView = db.UserViews.Where(a => a.UserID == UserID && a.View.ID == viewid_Product).FirstOrDefault();

            if (UserView != null && UserView.Role_Edit == true)
            {
                WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
                List<string> computerDetails = identity.Name.Split('\\').ToList();
                Employee item = db.Employees.Find(Cls_Employee.ID);
                item.Name = Cls_Employee.Name;
                item.Job_Id = Cls_Employee.Job_Id;
                item.Salary = Cls_Employee.Salary;
                item.Mobile = Cls_Employee.Mobile;
                item.ComputerName = computerDetails[0];
                item.ComputerUser = computerDetails[1];
                item.userid_Edit = UserID;
                item.EditDate = DateTime.Now;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                Error.ErrorName = "تمت الحفظ بنجاح ... جاري إعادة تحميل الصفحة";
                Error.ID = item.ID;
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Error.ErrorFullNumber = "ليس لديك صلاحية-401";
                Error.ErrorNumber = "401";
                Error.ErrorName = "عفوا ، ليس لديك صلاحية";
                return View("~/Views/Shared/ErrorPage.cshtml", Error);
            }
        }

        [HttpGet]
        public ActionResult loadEmployee(int id)
        {
            Cls_Employee Cls_Employee = new Cls_Employee();
            Employee Employee = db.Employees.Find(id);
            Cls_Employee.ID = Employee.ID;
            Cls_Employee.Name = Employee.Name;
            Cls_Employee.Job_Id = Employee.Job_Id;
            Cls_Employee.Job_Name = Employee.Job.Name;
            Cls_Employee.Salary = Employee.Salary;
            Cls_Employee.Mobile = Employee.Mobile;
            Cls_Employee.ComputerName = Employee.ComputerName;
            Cls_Employee.ComputerUser = Employee.ComputerUser;
            Cls_Employee.InDate = Employee.InDate.ToString("yyyy-MM-dd");
            Cls_Employee.User_Name = Employee.User.NAME;
            var list = JsonConvert.SerializeObject(Cls_Employee, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }
        [HttpGet]
        [CustomAuthorize(Roles = "الراتب&edit")]
        public ActionResult Salary()
        {
            return View();
        }


        [HttpGet]
        public ActionResult loadSalary(int id)
        {

            Cls_Salary Cls_Salary = new Cls_Salary();
            Cls_Salary.Cls_Employee_Salary = new List<Cls_Employee_Salary>();
            Salary Salary = db.Salaries.Find(id);
            Cls_Salary.ID = Salary.ID;
            Cls_Salary.Notes = Salary.Notes;
            Cls_Salary.DateFrom = Salary.DateFrom.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.GetCultureInfo("en-us", "en"));
            Cls_Salary.DateTO = Salary.DateFrom.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.GetCultureInfo("en-us", "en"));
            Cls_Salary.User_Name = Salary.User.NAME;
            Cls_Salary.ComputerName = Salary.ComputerName;
            Cls_Salary.ComputerUser = Salary.ComputerUser;
            Cls_Salary.InDate = Salary.InDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.GetCultureInfo("en-us", "en"));

            Cls_Salary.Cls_Employee_Salary = db.Employee_Salary.Select(a => new Cls_Employee_Salary
            {
                ID = a.ID,
                Salary_Id=a.Salary_Id,
                Employee_Id = a.Employee_Id,
                Name = a.Employee.Name,
                Salary = a.Salary,
                Job_Name = a.Employee.Job.Name,
                Mobile = a.Employee.Mobile
            })
               .ToList();
            var list = JsonConvert.SerializeObject(Cls_Salary, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Save_Salary(Cls_Salary Cls_Salary)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            int Invoice_Number = 0;
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";
            DateTime DateFrom = DateTime.ParseExact(Cls_Salary.DateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime DateTO = DateTime.ParseExact(Cls_Salary.DateTO, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();

            Invoice_Number = db.Salaries.Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            decimal TotalSalaries = Cls_Salary.Cls_Employee_Salary.Sum(a => a.Salary);
            Salary Salary = new Salary
            {
                Invoice_Number = Invoice_Number,
                DateFrom = DateFrom,
                DateTO = DateTO,
                Notes = Cls_Salary.Notes,
                Userid_In = UserID,
                TotalSalaries = TotalSalaries,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1],
                InDate = DateTime.Now,
            };
            db.Salaries.Add(Salary);
            db.SaveChanges();
            Invoice_Number = db.Masrofats.Where(a => a.Is_Deleted == false).Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
            Masrofat Masrofat = new Masrofat
            {
                Salary_ID = Salary.ID,
                Masrofat_Type_Id = 3, // راتب
                Invoice_Number = Invoice_Number,
                Date_Invoice = DateTime.Now,
                Money = TotalSalaries,
                Bian = "سداد رواتب رقم " + Salary.Invoice_Number.ToString() + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Salary.InDate.ToString("yyyy/MM/dd"))
                   + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Salary.TotalSalaries))
                   + " ريال ",
                Userid_In = UserID,
                InDate = DateTime.Now,
                ComputerName = computerDetails[0],
                ComputerUser = computerDetails[1]
            };
            db.Masrofats.Add(Masrofat);
            db.SaveChanges();
            foreach (var item in Cls_Salary.Cls_Employee_Salary)
            {
                Employee_Salary Employee_Salary = new Employee_Salary
                {
                    Salary_Id = Salary.ID,
                    Employee_Id = item.Employee_Id,
                    Salary = item.Salary,
                };
                db.Employee_Salary.Add(Employee_Salary);

            }
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Salary.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize]
        [HttpPost]
        public ActionResult Edit_Salary(Cls_Salary Cls_Salary)
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            
            System.Globalization.DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new System.Globalization.HijriCalendar();
            HijriDTFI.ShortDatePattern = "dd/MM/yyyy";
            DateTime DateFrom = DateTime.ParseExact(Cls_Salary.DateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime DateTO = DateTime.ParseExact(Cls_Salary.DateTO, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();

         
            decimal TotalSalaries = Cls_Salary.Cls_Employee_Salary.Sum(a => a.Salary);
            int ID = Cls_Salary.ID;
            Salary Salary = db.Salaries.Find(ID);


            Salary.DateFrom = DateFrom;
            Salary.DateTO = DateTO;
            Salary.Notes = Cls_Salary.Notes;
            Salary.userid_Edit = UserID;
            Salary.TotalSalaries = TotalSalaries;
            Salary.ComputerName = computerDetails[0];
            Salary.ComputerUser = computerDetails[1];
            Salary.EditDate = DateTime.Now;
            db.Entry(Salary).State = EntityState.Modified;
            db.SaveChanges();
            //******************************************************
            Masrofat Masrofat = db.Masrofats.Where(a => a.Salary_ID == ID).FirstOrDefault();
            if (Masrofat != null)
            {

                //Salary_ID = Salary.ID,
                Masrofat.Masrofat_Type_Id = 3; // راتب
                

                Masrofat.Money = TotalSalaries;
                Masrofat.Bian = "سداد رواتب رقم " + Salary.Invoice_Number.ToString() + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Salary.InDate.ToString("yyyy/MM/dd"))
                 + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Salary.TotalSalaries))
                 + " ريال ";
                db.Entry(Masrofat).State = EntityState.Modified;
                db.SaveChanges();

            }
            else
            {
                int Invoice_Number = db.Masrofats.Select(a => a.Invoice_Number).DefaultIfEmpty(0).Max() + 1;
                Masrofat = new Masrofat
                {
                    Salary_ID = Salary.ID,
                    Masrofat_Type_Id = 3, // راتب
                    Invoice_Number = Invoice_Number,
                    Date_Invoice = DateTime.Now,
                    Money = TotalSalaries,
                    Bian = "سداد رواتب رقم " + Salary.Invoice_Number.ToString() + " بتاريخ " + ConvertToEasternArabicNumerals.ConvertAR(Salary.InDate.ToString("yyyy/MM/dd"))
                      + " بملغ " + ConvertToEasternArabicNumerals.ConvertAR(String.Format("{0:0.##}", Salary.TotalSalaries))
                      + " ريال ",
                    Userid_In = UserID,
                    InDate = DateTime.Now,
                    ComputerName = computerDetails[0],
                    ComputerUser = computerDetails[1]
                };
                db.Masrofats.Add(Masrofat);
                db.SaveChanges();
            }

            //*******************************************************



            foreach (var item in Cls_Salary.Cls_Employee_Salary)
            {
                int _id = item.ID;
                Employee_Salary Employee_Salary = db.Employee_Salary.Find(_id);
                Employee_Salary.Employee_Id = item.Employee_Id;
                Employee_Salary.Salary = item.Salary;
                db.Entry(Employee_Salary).State = EntityState.Modified;
            }
            db.SaveChanges();
            Error.ErrorName = "تم الإضافة بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = Salary.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);
        }

    }
}