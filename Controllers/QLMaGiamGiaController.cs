using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniShopWebNHT.Models;
using System.ComponentModel;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using PagedList;
using System.Data.Entity;

namespace MiniShopWebNHT.Controllers
{
    public class QLMaGiamGiaController : Controller
    {
        // GET: QLMaGiamGia
    
        MiniShopEntities db = new MiniShopEntities();
        

        public class HttpParamActionAttribute : ActionNameSelectorAttribute
        {
            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                if (actionName.Equals(methodInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                var request = controllerContext.RequestContext.HttpContext.Request;
                return request[methodInfo.Name] != null;
            }
        }
        [HttpGet]
        public ActionResult Index(int? size, int? page, string sortProperty, string sortOrder, string searchString)
        {
            ViewBag.searchValue = searchString;
            ViewBag.sortProperty = sortProperty;
            ViewBag.page = page;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "25", Value = "25" });

            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.size = items;
            ViewBag.currentSize = size;

            var links = from l in db.MaGiams select l;
            // 5. T?o thu?c tính s?p x?p m?c d?nh là "LinkID"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "ID";

            // 5. S?p x?p tang/gi?m b?ng phuong th?c OrderBy s? d?ng trong thu vi?n Dynamic LINQ
            if (sortOrder == "desc") links = links.OrderBy(sortProperty + " desc");
            else if (sortOrder == "asc") links = links.OrderBy(sortProperty);
            else links = links.OrderBy("MaGiamGia");

            if (!String.IsNullOrEmpty(searchString))
            {
                links = links.Where(s => s.MaGiamGia.Contains(searchString));
            }

            page = page ?? 1;


            int pageSize = (size ?? 5);

            ViewBag.pageSize = pageSize;

            // 6. Toán t? ?? trong C# mô t? n?u page khác null thì l?y giá tr? page, còn
            // n?u page = null thì l?y giá tr? 1 cho bi?n pageNumber. --- dammio.com
            int pageNumber = (page ?? 1);

            // 6.2 L?y t?ng s? record chia cho kích thu?c d? bi?t bao nhiêu trang
            int checkTotal = (int)(links.ToList().Count / pageSize) + 1;
            // N?u trang vu?t qua t?ng s? trang thì thi?t l?p là 1 ho?c t?ng s? trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            return View(links.ToPagedList(pageNumber, pageSize));

        }
        [HttpPost, HttpParamAction]
        public ActionResult Reset()
        {
            ViewBag.searchValue = "";
            Index(null, null, "", "", "");
            return View();

        }
        public ActionResult EditMa(int? id)
        {

            //ViewBag.GiaTri = db.MaGiams.SingleOrDefault(n => n.ID == id).GiaTri;
            // List<Category> lis = db.Categories.ToList();

            MaGiam magiam = db.MaGiams.Find(id);
            return View(magiam);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMa([Bind(Include = "ID,MaGiamGia,GiaTri,NgayHetHan,SoLuong")] MaGiam magiam )
        {
            if (ModelState.IsValid)
            {
                db.Entry(magiam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(magiam);
        }
        public ActionResult DeleteMa(int? id)
        {
            MaGiam magiam = db.MaGiams.Find(id);
            return View(magiam);
        }
        [HttpPost, ActionName("DeleteMa")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaGiam magiam = db.MaGiams.Find(id);
            db.MaGiams.Remove(magiam);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        

        public ActionResult CreateMa()
        {
            
            return View();
        }
        //public string ProcessUpload(HttpPostedFileBase file)
        //{
        //    //xử lí upload
        //    file.SaveAs(Server.MapPath("/Assets/images" + file.FileName));
        //    return "/Assets/images" + file.FileName;
        //}
        //public string UploadEdit(HttpPostedFileBase file)
        //{
        //    //xử lí upload
        //    file.SaveAs(Server.MapPath("/Assets/images" + file.FileName));
        //    return file.FileName;
        //}
        [HttpPost]

        public ActionResult Tao(FormCollection frmTao, MaGiam magiam)
        {

            magiam.MaGiamGia = frmTao["MaGiamGia"];
            //magiam.GiaTri = frmTao["MoTa"];
            magiam.ID = Convert.ToInt32(frmTao["ID"]);
            //magiam. = Convert.ToInt32(frmTao["MaLoai"]);
            //sanpham.PubId = Convert.ToInt32(frmTao["pub"]);
            magiam.GiaTri = Convert.ToInt32(frmTao["GiaTri"]);
            magiam.SoLuong = Convert.ToInt32(frmTao["SoLuong"]);
            //sanpham.TrangThai = frmTao["TrangThai"];
            //sanpham.GioiTinh = frmTao["GioiTinh"];
            magiam.NgayHetHan = DateTime.Now;
            //sanpham.NgayChinhSua = DateTime.Now;
            //sanpham.HinhAnh = frmTao["fileUpload"];
            db.MaGiams.Add(magiam);
            db.SaveChanges();
            return RedirectToAction("Index", "QLMaGiamGia");
        }
    }
}