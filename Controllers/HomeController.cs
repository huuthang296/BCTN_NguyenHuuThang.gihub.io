using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniShopWebNHT.Models;

namespace MiniShopWebNHT.Controllers
{
    public class HomeController : Controller
    {
        MiniShopEntities db = new MiniShopEntities();
        public ActionResult TrangChu()
        {
            return View();
        }
        public ActionResult SanPham()
        {
            return View(db.SanPhams.Where(x => x.MaLoai == 1|| x.MaLoai== 2 || x.MaLoai == 3 || x.MaLoai == 4).ToList());
        }
        public ActionResult ThongTin()
        {
            return View();
        }
        public ActionResult TinTuc()
        {
            return View();
        }
        public ActionResult LienHe()
        {
            return View();
        }

        public ActionResult LoaiSP(int? MaLoai  ) 
        {

            
            List<SanPham> sp = db.SanPhams.Where(n => n.MaLoai == MaLoai).ToList();
           
            return View(sp);

           
        }






    }
}