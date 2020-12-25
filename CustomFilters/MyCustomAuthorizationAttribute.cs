using Warehouse.Models;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Warehouse.CustomFilters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        
        private DB_StoreEntities db = new DB_StoreEntities();
        public string UsersConfigKey { get; set; }
        public string RolesConfigKey { get; set; }
      
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            
            var authorizedRoles = ConfigurationManager.AppSettings[RolesConfigKey];
            Roles = String.IsNullOrEmpty(Roles) ? authorizedRoles : Roles;
            //bool authorize = false;
            string NAME = System.Web.HttpContext.Current.User.Identity.Name;
            if (NAME == "manager")
            {
                return true;
            }
            else
            {
                if (Roles == "" )
                {
                    if (NAME != "")
                    {
                        return true;
                    }
                    else {
                        return false;
                    }
                }

                string[] _RolesArr = Roles.ToString().Split('&');
                bool ENTER = false, SAVE = false, Role_Edit = false, DEL = false;
                foreach (string word in _RolesArr)
                {
                    if (word == "enter")
                        ENTER = true;
                    if (word == "save")
                        SAVE = true;
                    else if (word == "edit")
                        Role_Edit = true;
                    else if (word == "delete")
                        DEL = true;
                }

                string[] _Roles_Account_Type = Roles.ToString().Split('$');
                int Account_Type = -1;
                foreach (string word in _Roles_Account_Type)
                {
                    if (word == "1")
                        Account_Type = 1;
                    if (word == "0")
                        Account_Type = 0;
                }
                User User = db.Users.Where(a => a.NAME == NAME).FirstOrDefault();
                if (User != null)
                {
                    if (Account_Type != -1)
                    {
                        string roleName = _RolesArr[0];
                        View View = db.Views.Where(a => a.Name == roleName).FirstOrDefault();
                        if (View != null)
                        {
                            decimal UserID = User.ID;
                            decimal ViewID = View.ID;
                            if (db.UserViews.Where(a => a.UserID == UserID && a.ViewID == ViewID && a.Role_Enter == true).Any())
                            {
                                // return true;
                                if (User.Account_Type == Account_Type)
                                {
                                    return true;
                                }
                                else {
                                    return false;
                                }
                            }
                            else { return false; }
                        }
                        else { return false; }
                    }
                    else if (ENTER)
                    {
                        string roleName = _RolesArr[0];
                        View View = db.Views.Where(a => a.Name == roleName).FirstOrDefault();
                        if (View != null)
                        {
                            decimal UserID = User.ID;
                            decimal ViewID = View.ID;
                            if (db.UserViews.Where(a => a.UserID == UserID && a.ViewID == ViewID && a.Role_Enter == true).Any())
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else { return false; }
                    }
                    else if (SAVE)
                    {
                        string roleName = _RolesArr[0];
                        View View = db.Views.Where(a => a.Name == roleName).FirstOrDefault();
                        if (View != null)
                        {
                            decimal UserID = User.ID;
                            decimal ViewID = View.ID;
                            if (db.UserViews.Where(a => a.UserID == UserID && a.ViewID == ViewID && a.Role_Save == true).Any())
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else { return false; }
                    }
                    else if (Role_Edit)
                    {
                        string roleName = _RolesArr[0];
                        View View = db.Views.Where(a => a.Name == roleName).FirstOrDefault();
                        if (View != null)
                        {
                            decimal UserID = User.ID;
                            decimal ViewID = View.ID;
                            if (db.UserViews.Where(a => a.UserID == UserID && a.ViewID == ViewID && a.Role_Edit == true).Any())
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else { return false; }
                       
                    }
                    else if (DEL)
                    {
                        string roleName = _RolesArr[0];
                        View View = db.Views.Where(a => a.Name == roleName).FirstOrDefault();
                        if (View != null)
                        {
                            decimal UserID = User.ID;
                            decimal ViewID = View.ID;
                            if (db.UserViews.Where(a => a.UserID == UserID && a.ViewID == ViewID && a.Role_Delete == true).Any())
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else { return false; }
                       
                    }
                    else
                    {
                        View View = db.Views.Where(a => a.Name == Roles).FirstOrDefault();
                        if (View != null)
                        {
                            decimal UserID = User.ID;
                            decimal ViewID = View.ID;
                            if (db.UserViews.Where(a => a.UserID == UserID && a.ViewID == ViewID).Any())
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else { return false; }
                    }
                }
                else { return false; }
            }
           
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();

            //filterContext.Result = new RedirectResult("~/Home/Unauthorized");
            //filterContext.Result = new ViewResult { ViewName = "Unauthorized" };

            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            //    base.HandleUnauthorizedRequest(filterContext);
            //else
            //{
            //    filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
            //}
        }
    }

}

//foreach (var role in allowedroles)
//{
//    var user = context.AppUser.Where(m => m.UserID == GetUser.CurrentUser/* getting user form current context */ && m.Role == role &&
//    m.IsActive == true); // checking active users with allowed roles.  
//    if (user.Count() > 0)
//    {
//        authorize = true; /* return true if Entity has current user(active) with specific role */
//    }
//}