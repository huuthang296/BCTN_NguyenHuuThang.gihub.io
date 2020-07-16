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


namespace MiniShopWebNHT.Areas.Admin.Controllers
{
    public class AdminCRUDController : Controller
    {
        MiniShopEntities db = new MiniShopEntities();
        // GET: AdminCRUD
        
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

            var links = from l in db.SanPhams select l;
            // 5. T?o thu?c tính s?p x?p m?c d?nh là "LinkID"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "MaSP";

            // 5. S?p x?p tang/gi?m b?ng phuong th?c OrderBy s? d?ng trong thu vi?n Dynamic LINQ
            if (sortOrder == "desc") links = links.OrderBy(sortProperty + " desc");
            else if (sortOrder == "asc") links = links.OrderBy(sortProperty);
            else links = links.OrderBy("TenSP");

            if (!String.IsNullOrEmpty(searchString))
            {
                links = links.Where(s => s.TenSP.Contains(searchString));
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
        public ActionResult Edit(int? id)
        {

            ViewBag.HinhAnh = db.SanPhams.SingleOrDefault(n => n.MaSP == id).HinhAnh;
            // List<Category> lis = db.Categories.ToList();

            SanPham sanpham = db.SanPhams.Find(id);
            return View(sanpham);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSP,TenSP,DonGia,MoTa,MaLoai,TrangThai,HinhAnh,SoLuong,GioiTinh,NgayChinhSua,NgayTao")] SanPham sanpham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanpham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sanpham);
        }
        public ActionResult Delete(int? id)
        {
            SanPham sanpham = db.SanPhams.Find(id);
            return View(sanpham);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanpham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanpham);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChiTietSanPhamAdmin(int? MaSP)

        {
            //  Book s = db.Books.Single(n => n.BookId == id);
            SanPham b = (from p in db.SanPhams where p.MaSP == MaSP select p).ToArray()[0];

            return View(b);
        }

        public ActionResult Create()
        {
            List<LoaiSP> LSP = db.LoaiSPs.ToList();
            SelectList ListLoaiSP = new SelectList(LSP, "MaLoai", "TenLoai");
            //List<Category> cate = db.Categories.ToList();
            //SelectList ListCate = new SelectList(cate, "CateId", "CateName");
            //List<Author> au = db.Authors.ToList();
            //SelectList ListAuthor = new SelectList(au, "AuthorId", "AuthorName");
            //ViewBag.AuthorList = ListAuthor;
            //ViewBag.CategoryList = ListCate;
            ViewBag.LoaiSPList = ListLoaiSP;
            return View();
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            //xử lí upload
            file.SaveAs(Server.MapPath("/Assets/images" + file.FileName));
            return "/Assets/images" + file.FileName;
        }
        public string UploadEdit(HttpPostedFileBase file)
        {
            //xử lí upload
            file.SaveAs(Server.MapPath("/Assets/images" + file.FileName));
            return file.FileName;
        }
        [HttpPost]

        public ActionResult Tao(FormCollection frmTao, SanPham sanpham)
        {

            sanpham.TenSP = frmTao["TenSP"];
            sanpham.MoTa = frmTao["MoTa"];
            sanpham.MaSP = Convert.ToInt32(frmTao["MaSP"]);
            sanpham.MaLoai = Convert.ToInt32(frmTao["MaLoai"]);
            //sanpham.PubId = Convert.ToInt32(frmTao["pub"]);
            sanpham.DonGia = Convert.ToDecimal(frmTao["DonGia"]);
            sanpham.SoLuong = Convert.ToInt32(frmTao["SoLuong"]);
            sanpham.TrangThai = frmTao["TrangThai"];
            sanpham.GioiTinh = frmTao["GioiTinh"];
            sanpham.NgayTao = DateTime.Now;
            sanpham.NgayChinhSua = DateTime.Now;
            sanpham.HinhAnh = frmTao["fileUpload"];
            db.SanPhams.Add(sanpham);
            db.SaveChanges();
            return RedirectToAction("Index", "AdminCRUD");
        }

        public ActionResult createLoaiSP()
        {
            var f = from s in db.LoaiSPs select s;
            ViewBag.sklist = db.LoaiSPs.ToList();
            return View();

        }
        [HttpPost]
        public ActionResult createLoaiSP(FormCollection frmCreate, LoaiSP a)
        {

            a.TenLoai = frmCreate["TenLoai"];
            a.MaLoai = Convert.ToInt32(frmCreate["MaLoai"]);

            db.LoaiSPs.Add(a);
            db.SaveChanges();
            return RedirectToAction("CreateLoaiSP", "AdminCRUD");
        }
        //public ActionResult createPub()
        //{
        //    var f = from s in db.Publishers select s;
        //    ViewBag.sklist = db.Publishers.ToList();
        //    return View();

        //}
        //[HttpPost]
        //public ActionResult createPub(FormCollection frmCreate, Publisher p)
        //{

        //    p.Name = frmCreate["Name"];
        //    p.Description = frmCreate["Description"];

        //    db.Publishers.Add(p);
        //    db.SaveChanges();
        //    return RedirectToAction("createPub", "AdminCRUD");
        //}
        //public ActionResult createCate()
        //{
        //    var f = from s in db.Categories select s;
        //    ViewBag.sklist = db.Categories.ToList();
        //    return View();

        //}
        //[HttpPost]
        //public ActionResult createCate(FormCollection frmCreate, Category c)
        //{

        //    c.CateName = frmCreate["CateName"];
        //    c.Description = frmCreate["Description"];

        //    db.Categories.Add(c);
        //    db.SaveChanges();
        //    return RedirectToAction("createCate", "AdminCRUD");
        //}
    }
}