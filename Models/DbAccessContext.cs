using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MiniShopWebNHT.Models
{
    public class DbAccessContext :DbContext
    {
        public DbAccessContext() : base("DefaultConnection") { }
        public DbSet<Employee> Employees
        {
            get;
            set;
        }
        public DbSet<Department> Departments
        {
            get;
            set;
        }
    }
}