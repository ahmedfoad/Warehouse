using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;

namespace Warehouse.Models.Administration
{
    public enum _Action
    {
        دخول = 0,
        حفظ = 1,
        تعديل = 2,
        حذف = 3,
        استعلام = 4,
        طباعة = 5,
        تقرير = 6,
        أرشفة = 7,
        خروج=8
    }
    public enum Role
    {
        مديرالنظام = 1,
        مستخدم = 0,
    }
  
}