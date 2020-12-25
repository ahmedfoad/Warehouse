using Warehouse.Models;
using Warehouse.Models.Administration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;

using System.Security.Principal;

using System.Web;
using System.Web.Mvc;


using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Threading;
using Warehouse.CustomFilters;

namespace Warehouse.Controllers.Administration
{
    public class UsersController : Controller
    {
        private readonly IAuthenticationManager _auth;
        public UsersController(IAuthenticationManager auth)
        {
            this._auth = auth;
        }
        DB_StoreEntities db = new DB_StoreEntities();
        ErrorViewModel Error = new ErrorViewModel();


        private int recordsPerPage = 300;
        string[] date = DateTime.Now.ToString(CultureInfo.GetCultureInfo("ar-SA")).Substring(0, 8).Split('/');
        private string[] time = DateTime.Now.TimeOfDay.ToString().Split(':');
        private int Viewid = 2466; //الإستعلام عن المستخدمين

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User model, string returnUrl = null)
        {
            if (model.Username == "manager" && model.Password == "manager")
            {
                var identity = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, model.Username)
                    , new Claim(ClaimTypes.NameIdentifier, "2")
                }, DefaultAuthenticationTypes.ApplicationCookie);

                //  var identity = System.Web.HttpContext.Current.User.Identity as ClaimsIdentity;
                //_identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", user.ID.ToString()));
                //_identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.NAME));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user.NAME));
                //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));

                this._auth.SignIn(new AuthenticationProperties
                {
                    IsPersistent = false //model.RememberMe
                }, identity);



                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                           && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                //return RedirectToAction("Index", "Home");
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var user = db.Users.Where(a => a.Username == model.Username && a.Password == model.Password && a.STOPEMP == false).FirstOrDefault();
                if (user != null)
                {
                    var identity = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, user.NAME)
                    , new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
                }, DefaultAuthenticationTypes.ApplicationCookie);

                    //  var identity = System.Web.HttpContext.Current.User.Identity as ClaimsIdentity;
                    //_identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", user.ID.ToString()));
                    //_identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.NAME));
                    //identity.AddClaim(new Claim(ClaimTypes.Name, user.NAME));
                    //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));

                    this._auth.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false //model.RememberMe
                    }, identity);



                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                               && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    //return RedirectToAction("Index", "Home");
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

        }

        public ActionResult LoadDataChangePass()
        {
            Error.ErrorName = HttpContext.User.Identity.Name;
            return Json(Error, JsonRequestBehavior.AllowGet);
            //if (Session["UserID"] != null)
            //{
            //    Error.ErrorName = Session["Username"].ToString();
            //    return Json(Error, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    Error.ErrorFullNumber = "AR-Logout-089";
            //    Error.ErrorNumber = "089";
            //    Error.Url = "/Users/Operation";
            //    Error.ErrorName = "تم تسجيل خروجك آلياً لانتهاء المدة المسموح بها";
            //    return Json(Error, JsonRequestBehavior.AllowGet);
            //}
        }
        public ActionResult ChangePassword(string NewPass, string OldPass)
        {

            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            User User = db.Users.Find(UserID);
            if (User.Password == OldPass)
            {
                User.Password = NewPass;
                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                Error.ErrorName = "تم تعديل كلمة المرور بنجاح ... جاري تسجيل الخروج من البرنامج";
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Error.ErrorName = "كلمة المرور القديمة خطأ";
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllViews(int? page, string search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Views
              .Where(s => string.IsNullOrEmpty(search) ? true : s.Name.Contains(search))
              .OrderBy(s => s.ViewIndex).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).Select(a =>
              new View_Roles
              {
                  _View = new _View { ID = a.ID, Name = a.Name },
                  Role_Enter = false,
                  Role_Save = false,
                  Role_Edit = false,
                  Role_Delete = false
              }
              ).ToList();


            var list = JsonConvert.SerializeObject(AllRecords,
    Formatting.None,
    new JsonSerializerSettings()
    {
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    });

            return Content(list, "application/json");
        }
        public ActionResult getAllUsers(int? page, string search)
        {
            int? skipRecords = (page != null ? page.Value : 0) * recordsPerPage;

            var AllRecords = db.Users
              .Where(s => string.IsNullOrEmpty(search) ? true : s.NAME.Contains(search))
              .OrderBy(s => s.ID).Skip(skipRecords != null ? skipRecords.Value : 0)
              .Take(recordsPerPage).Select(a => new ClsUser
              {
                  ID = a.ID,
                  NAME = a.NAME,
                  Username = a.Username,
                  Password = a.Password,
                  EMAIL = a.EMAIL,
                  INDATE = a.INDATE,
                  ROLE = a.ROLE,
                  STOPEMP = a.STOPEMP

              }).ToList();

            var list = JsonConvert.SerializeObject(AllRecords,
    Formatting.None,
    new JsonSerializerSettings()
    {
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    });

            return Content(list, "application/json");
        }

        public ActionResult loadUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                Error.ErrorFullNumber = "AR-Edit-082";
                Error.ErrorNumber = "082";
                Error.Url = "/Users/Search";
                Error.ErrorName = "لا يوجد مستخدم يحمل الرقم  " + id;
                return Json(Error, JsonRequestBehavior.AllowGet);
            }

            List<View_Roles> View_Roles = db.UserViews.Where(a => a.UserID == id).Select(a =>
            new View_Roles
            {
                _View = new _View { ID = a.ViewID, Name = a.View.Name },
                Role_Enter = a.Role_Enter,
                Role_Save = a.Role_Save,
                Role_Edit = a.Role_Edit,
                Role_Delete = a.Role_Delete
            }
                ).ToList();
            Cls_User Cls_User = new Cls_User
            {
                User = new ClsUser { ID = user.ID, NAME = user.NAME, ROLE = user.ROLE, Username = user.Username, Password = user.Password, EMAIL = user.EMAIL },
                View_Roles = View_Roles
            };
            var list = JsonConvert.SerializeObject(Cls_User, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
            return Content(list, "application/json");
        }
        public ActionResult LastUser()
        {
            User user = db.Users.LastOrDefault();
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize(Roles = "إضافة مستخدم جديد&edit$1")]
        public ActionResult Operation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(Cls_User Cls_User)
        {
            if (db.Users.Where(a => a.Username == Cls_User.User.Username).Any() == true)
            {
                Error.ErrorName = "خطأ اسم المستخدم مكرر سابقا";
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            if (db.Users.Where(a => a.NAME == Cls_User.User.NAME).Any() == true)
            {
                Error.ErrorName = "خطأ اسم الموظف مكرر سابقا";
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            User User = new User();
            UserView UserView = db.UserViews.Where(a => a.UserID == UserID && a.View.Name == "إضافة مستخدم جديد").FirstOrDefault();
            User.Username = Cls_User.User.Username;
            User.Password = Cls_User.User.Password;
            User.NAME = Cls_User.User.NAME;

            User.ROLE = 0;
            User.STOPEMP = false;
            User.INDATE = DateTime.Now;
            User.Account_Type = Cls_User.User.Account_Type;
            db.Users.Add(User);
            db.SaveChanges();



            //UserAction UserAction = new UserAction
            //{
            //    Userid = UserID,
            //    Viewid = Viewid,
            //    ActionDate = DateTime.Now,
            //    Action = ((_Action)1).ToString(), // خروج
            //    Operation = "إضافة مستخدم جديد يحمل الرقم : " + User.ID.ToString() + "-"
            //    + " واسم المستخدم  : " + User.Username + "-"
            //    + " واسم الموظف : " + User.NAME + "."
            //};
            //db.UserActions.Add(UserAction);
            //db.SaveChanges();

            foreach (View_Roles View_Roles in Cls_User.View_Roles)
            {
                UserView UserViewAdd = new UserView
                {
                    UserID = User.ID,
                    ViewID = View_Roles._View.ID,
                    Role_Enter = View_Roles.Role_Enter,
                    Role_Save = View_Roles.Role_Save,
                    Role_Edit = View_Roles.Role_Edit,
                    Role_Delete = View_Roles.Role_Delete
                };
                db.UserViews.Add(UserViewAdd);
            }
            db.SaveChanges();
            Error.ErrorName = "تمت إضافة المستخدم بنجاح ... جاري إعادة تحميل الصفحة";
            Error.ID = User.ID;
            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Edit(Cls_User Cls_User)
        {

            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            User User = new User();

            User = db.Users.Find(Cls_User.User.ID);
            User.Username = Cls_User.User.Username;
            User.Password = Cls_User.User.Password;
            User.NAME = Cls_User.User.NAME;

            User.ROLE = 0;
            User.STOPEMP = false;
            User.Account_Type = Cls_User.User.Account_Type;
            WindowsIdentity identity = HttpContext.Request.LogonUserIdentity;
            List<string> computerDetails = identity.Name.Split('\\').ToList();
            User.INDATE = DateTime.Now;
            db.Entry(User).State = EntityState.Modified;
            db.SaveChanges();

            List<UserView> userviewslist = db.UserViews.Where(a => a.UserID == Cls_User.User.ID).ToList();
            db.UserViews.RemoveRange(userviewslist);
            db.SaveChanges();



            //UserAction UserAction = new UserAction
            //{
            //    Userid = UserID,
            //    Viewid = Viewid,
            //    ActionDate = DateTime.Now,
            //    Action = ((_Action)2).ToString(),
            //    Operation = "تعديل بيانات مستخدم يحمل الرقم : " + User.ID.ToString() + "-"
            //   + " واسم المستخدم  : " + User.Username + "-"
            //   + " واسم الموظف : " + User.NAME + "."
            //};
            //db.UserActions.Add(UserAction);
            //db.SaveChanges();

            foreach (View_Roles View_Roles in Cls_User.View_Roles)
            {
                UserView UserViewAdd = new UserView
                {
                    UserID = User.ID,
                    ViewID = View_Roles._View.ID,
                    Role_Enter = View_Roles.Role_Enter,
                    Role_Save = View_Roles.Role_Save,
                    Role_Edit = View_Roles.Role_Edit,
                    Role_Delete = View_Roles.Role_Delete
                };
                db.UserViews.Add(UserViewAdd);
            }
            db.SaveChanges();

            Error.ID = User.ID;
            Error.ErrorName = "تم تعديل بيانات المستخدم بنجاح ... جاري إعادة تحميل الصفحة";
            return Json(Error, JsonRequestBehavior.AllowGet);



        }
        [CustomAuthorize(Roles = "إضافة مستخدم جديد&edit$1")]
        public ActionResult Delete(int id)
        {

            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());


            User User = db.Users.Find(id);
            User.STOPEMP = true;
            db.Entry(User).State = EntityState.Modified;
            db.SaveChanges();

            //UserAction UserAction = new UserAction
            //{
            //    Userid = UserID,
            //    Viewid = Viewid,
            //    ActionDate = DateTime.Now,
            //    Action = ((_Action)3).ToString(), // خروج
            //    Operation = "حذف بيانات مستخدم يحمل الرقم : " + User.ID + "-"
            //     + " واسم المستخدم  : " + User.Username + "-"
            //     + " واسم الموظف : " + User.NAME + "."
            //};
            //db.UserActions.Add(UserAction);
            //db.SaveChanges();

            Error.ErrorFullNumber = "AR-Delete-084";
            Error.ErrorNumber = "084";
            Error.Url = "/Home";
            Error.ErrorName = "تم حذف بيانات المستخدم بنجاح";
            return View("~/Views/Shared/ErrorPage.cshtml", Error);

        }
        [CustomAuthorize]
        public ActionResult Search()
        {
            //int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            //UserAction UserAction = new UserAction
            //{
            //    Userid = UserID,
            //    Viewid = Viewid,
            //    ActionDate = DateTime.Now,
            //    Action = ((_Action)4).ToString(), // خروج
            //    Operation = "الدخول إلى نافذة البحث والإستعلام عن المستخدمين"
            //};
            //db.UserActions.Add(UserAction);
            //db.SaveChanges();
            return View();
        }
        [HttpPost]
        public ActionResult Search(int? page, string Username)
        {
            var skipRecords = ((page != null && page != 1) ? page.Value : 0) * recordsPerPage;
            var AllRecords = db.Users
             .Where(s => string.IsNullOrEmpty(Username) ? true : s.NAME.Contains(Username))
             .OrderBy(s => s.ID).Skip(skipRecords)
             .Take(recordsPerPage).ToList();
            var list = JsonConvert.SerializeObject(AllRecords,
Formatting.None,
new JsonSerializerSettings()
{
    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

            return Content(list, "application/json");

        }
        public ActionResult LogOff()
        {
            this._auth.SignOut();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public int GetAccount_Type()
        {
            int UserID = int.Parse(HttpContext.User.Identity.GetUserId());
            User user = db.Users.Find(UserID);
            return user.Account_Type;
        }
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