using Warehouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Repositories
{
    public interface IAccount
    {
        IEnumerable<Warehouse.Models.User> getUsers();
    }
}
