using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniShopWebNHT.Models
{
    
    public partial class MyDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }

        //internal static object OrderByDescending(Func<object, object> p)
        //{
        //    throw new NotImplementedException();
        //}
    }
}