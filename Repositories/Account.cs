using Warehouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warehouse.Repositories
{
    public class Account : IAccount
    {

        public IEnumerable<Warehouse.Models.User> getUsers()
        {
            
            DB_StoreEntities db = new DB_StoreEntities();
            List<Warehouse.Models.User> users = db.Users.ToList();

            return users;
        }

        
    }
}