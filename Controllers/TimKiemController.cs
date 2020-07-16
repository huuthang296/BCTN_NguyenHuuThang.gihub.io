using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniShopWebNHT.Models;

namespace MiniShopWebNHT.Controllers
{
    public class TimKiemController : Controller
    {
        MiniShopEntities db = new MiniShopEntities();
        // GET: TimKiem
        public ActionResult TimKiem(string name)
        {
            List<SanPham> sp = db.SanPhams.Where(n => n.TenSP.Contains(name)).ToList();
            Session["nametimkiem"] = name;
            return View(sp);
        }
    }
}