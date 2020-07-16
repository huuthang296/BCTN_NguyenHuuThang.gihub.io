using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using MiniShopWebNHT.Models;
using System.Threading;

namespace MiniShopWebNHT.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang

        MiniShopEntities db = new MiniShopEntities();
        public List<SanPhamGH> LayGioHang()
        {
            //Session["NgayGiaoHang"] = DateTime.Parse(DateTime.Now.ToString());
            //Session["NgayNhanHang"] = DateTime.Parse(DateTime.Now.AddDays(+5).ToString("dd-MM-yyyy"));
            List<SanPhamGH> lstSP = Session["GioHang"] as List<SanPhamGH>;
            if (lstSP == null)
            {
                lstSP = new List<SanPhamGH>();
                Session["GioHang"] = lstSP;
            }
            return lstSP;
        }

        public ActionResult GioHang()
        {
            if (Session["username"] == null && Session["Email"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else
            {
                //List<KHACHHANG> lstKH
                List<SanPhamGH> listSP = LayGioHang();
                int TongSL = 0;
                double TongTien = 0;

                foreach (var item in listSP)
                {
                    TongSL += item.SoLuongMua;
                    TongTien += item.TongTien;
                    var tongtien1 = String.Format("{0:N0}", TongTien);
                    ViewBag.tongtien = tongtien1;
                }
                if (Session["MaGiam"] != null)
                {
                    int vat = (int)Session["MaGiam"];
                    TongTien = TongTien - TongTien * vat / 100;
                    var tongtien = String.Format("{0:N0}", TongTien);
                    Session["TongSL"] = TongSL.ToString();
                    Session["TongTien"] = tongtien.ToString();
                }
                else
                {
                    var tongtien = String.Format("{0:N0}", TongTien);
                    Session["TongSL"] = TongSL.ToString();
                    Session["TongTien"] = tongtien.ToString();
                }

                return View(listSP);
            }
        }

        [HttpPost]
        public ActionResult ThemGioHang(int iMaSP, int? SL)
        {

            List<SanPhamGH> lstSP = LayGioHang();
            SanPhamGH SP = lstSP.Find(n => n.MaSP == iMaSP);
            if (SP == null)
            {
                SP = new SanPhamGH();
                SanPham sp = db.SanPhams.Single(n => n.MaSP == iMaSP);
                SP.MaSP = iMaSP;
                SP.TenSP = sp.TenSP;
                SP.HinhAnh = sp.HinhAnh;
                SP.DonGia = double.Parse(sp.DonGia.ToString());
                if (SL == null)
                {
                    SP.SoLuongMua = 1;
                }
                else
                {
                    SP.SoLuongMua = int.Parse(SL.ToString());
                }
                lstSP.Add(SP);
                Session["GioHang"] = lstSP;

                return Json(lstSP, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (SL == null)
                {
                    SP.SoLuongMua++;
                }
                else
                {
                    SP.SoLuongMua = int.Parse(SL.ToString());
                }
                Session["GioHang"] = lstSP;
                return Json(lstSP, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult XoaSP(int iMaSP)
        {
            List<SanPhamGH> lstSP = LayGioHang();
            SanPhamGH SP = lstSP.Find(n => n.MaSP == iMaSP);
            lstSP.Remove(SP);
            Session["GioHang"] = lstSP;
            return Json(lstSP, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GioHang(FormCollection frm, DonHang donhang)
        {
            //Session["NgayGiaoHang"] = DateTime.Parse(DateTime.Now.ToString());
            //Session["NgayNhanHang"] = DateTime.Parse(DateTime.Now.AddDays(+5).ToString("dd-MM-yyyy"));
            int x = 0;
            string danggiao = "Đang giao";
            string ngaygiao = "2019/12/1";
            if (Session["Email"] == null && Session["username"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else
            {

                if (Session["username"] != null)
                {
                    x = int.Parse(Session["makh"].ToString());
                }
                else
                {
                    x = int.Parse(Session["MaKH"].ToString());
                }
                var user = db.KhachHangs.FirstOrDefault(n => n.MaKH == x);

                if (user.DiaChiKH == null || user.SoDienThoaiKH == null)
                {
                    user = db.KhachHangs.Find(x);
                    {
                        user.SoDienThoaiKH = frm["dienthoainhanhang"];
                        user.DiaChiKH = frm["diachinhanhang"];

                    };
                    if (Session["username"] != null)
                    {
                        db.Entry(user);
                        donhang.MaKH = int.Parse(Session["makh"].ToString());
                        donhang.NgayDatHang = DateTime.Parse(DateTime.Now.ToString());
                        //donhang.NgayGiaoHang = ngaygiao;
                        donhang.TongTien = decimal.Parse(Session["TongTien"].ToString());
                        donhang.TrangThai = danggiao;
                        donhang.TenNguoiNhan = frm["tennguoinhan"];
                        donhang.DienThoaiKH = frm["dienthoainhanhang"];
                        donhang.DiaChiNhan = frm["diachinhanhang"];
                        donhang.EmailNhanHang = frm["email"];
                        db.DonHangs.Add(donhang);
                        db.SaveChanges();
                        List<SanPhamGH> listSP = LayGioHang();
                        foreach (var item in listSP)
                        {
                            ChiTietDH ctdh = new ChiTietDH();
                            ctdh.SoDH = donhang.SoHD;
                            ctdh.MaSP = item.MaSP;
                            ctdh.SoLuong = item.SoLuongMua;
                            ctdh.DonGia = (decimal)item.DonGia;
                            db.ChiTietDHs.Add(ctdh);
                            db.SaveChanges();
                        }
                        Session["GioHang"] = null;
                        return RedirectToAction("ThanhToanThanhCong", "GioHang");
                    }
                    else
                    {
                        db.Entry(user);
                        donhang.MaKH = int.Parse(Session["MaKH"].ToString());
                        donhang.NgayDatHang = DateTime.Parse(DateTime.Now.ToString());
                        //donhang.NgayGiaoHang = ngaygiao;
                        donhang.TongTien = decimal.Parse(Session["TongTien"].ToString());
                        donhang.TrangThai = danggiao;
                        donhang.TenNguoiNhan = frm["tennguoinhan"];
                        donhang.DienThoaiKH = frm["dienthoainhanhang"];
                        donhang.DiaChiNhan = frm["diachinhanhang"];
                        donhang.EmailNhanHang = frm["email"];
                        db.DonHangs.Add(donhang);
                        db.SaveChanges();
                        List<SanPhamGH> listSP = LayGioHang();
                        foreach (var item in listSP)
                        {
                            ChiTietDH ctdh = new ChiTietDH();
                            ctdh.SoDH = donhang.SoHD;
                            ctdh.MaSP = item.MaSP;
                            ctdh.SoLuong = item.SoLuongMua;
                            ctdh.DonGia = (decimal)item.DonGia;
                            db.ChiTietDHs.Add(ctdh);
                            db.SaveChanges();

                        }

                        Session["GioHang"] = null;
                        return RedirectToAction("ThanhToanThanhCong", "GioHang");
                    }
                }
                else
                {
                    if (Session["username"] != null)
                    {
                        donhang.MaKH = int.Parse(Session["makh"].ToString());
                        donhang.NgayDatHang = DateTime.Parse(DateTime.Now.ToString());
                        //donhang.NgayGiaoHang = DateTime.Parse(DateTime.Now.AddDays(+5).ToString("dd-MM-yyyy"));
                        donhang.TongTien = decimal.Parse(Session["TongTien"].ToString());
                        donhang.TrangThai = danggiao;
                        donhang.TenNguoiNhan = frm["tennguoinhan"];
                        donhang.DienThoaiKH = frm["dienthoainhanhang"];
                        donhang.DiaChiNhan = frm["diachinhanhang"];
                        donhang.EmailNhanHang = frm["email"];
                        db.DonHangs.Add(donhang);
                        db.SaveChanges();
                        List<SanPhamGH> listSP = LayGioHang();
                        foreach (var item in listSP)
                        {
                            ChiTietDH ctdh = new ChiTietDH();
                            ctdh.SoDH = donhang.SoHD;
                            ctdh.MaSP = item.MaSP;
                            ctdh.SoLuong = item.SoLuongMua;
                            ctdh.DonGia = (decimal)item.DonGia;
                            db.ChiTietDHs.Add(ctdh);
                            db.SaveChanges();

                        }

                        Session["Madh"] = donhang.SoHD;
                        return RedirectToAction("ThanhToanThanhCong", "GioHang");
                    }
                    else
                    {
                        donhang.MaKH = int.Parse(Session["MaKH"].ToString());
                        donhang.NgayDatHang = DateTime.Parse(DateTime.Now.ToString());
                        //donhang.NgayGiaoHang = DateTime.Parse(DateTime.Now.AddDays(+5).ToString("dd-MM-yyyy"));
                        donhang.TongTien = decimal.Parse(Session["TongTien"].ToString());
                        donhang.TrangThai = danggiao;
                        donhang.TenNguoiNhan = frm["tennguoinhan"];
                        donhang.DienThoaiKH = frm["dienthoainhanhang"];
                        donhang.DiaChiNhan = frm["diachinhanhang"];
                        donhang.EmailNhanHang = frm["email"];
                        db.DonHangs.Add(donhang);
                        db.SaveChanges();
                        List<SanPhamGH> listSP = LayGioHang();
                        foreach (var item in listSP)
                        {
                            ChiTietDH ctdh = new ChiTietDH();
                            ctdh.SoDH = donhang.SoHD;
                            ctdh.MaSP = item.MaSP;
                            ctdh.SoLuong = item.SoLuongMua;
                            ctdh.DonGia = (decimal)item.DonGia;
                            db.ChiTietDHs.Add(ctdh);
                            db.SaveChanges();

                        }

                        Session["Madh"] = donhang.SoHD;
                        return RedirectToAction("ThanhToanThanhCong", "GioHang");
                    }

                }

            }

        }
        public ActionResult ThanhToanThanhCong()
        {
            @Session["TongTien"] = null;
            @Session["TongSL"] = null;

            int x = int.Parse(Session["Madh"].ToString());
            List<SanPhamGH> listSP = LayGioHang();
            List<DonHang> dh = db.DonHangs.Where(n => n.SoHD == x).ToList();
            ViewData["dondathang"] = dh;
            return View(listSP);
        }
        public ActionResult ThanhToan()
        {
            int x = int.Parse(Session["makh"].ToString());
            List<KhachHang> KH = db.KhachHangs.Where(n => n.MaKH == x).ToList();
            return View(KH);
        }
        public ActionResult MaGiam(string magiam)
        {
            var Magiam = db.MaGiams.Where(x => x.MaGiamGia == magiam).FirstOrDefault();
            if (Magiam.SoLuong > 0 && Magiam != null)
            {
                MaGiam dh = db.MaGiams.Find(Magiam.ID);
                Session["MaGiam"] = Magiam.GiaTri;
                Session["soluongma"] = null;
                dh.SoLuong--;
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GioHang");
            }
            else if (Magiam.SoLuong <= 0 || Magiam != null)
            {
                Session["soluongma"] = 0;
                return RedirectToAction("GioHang");
            }
            else
            {
                return View();
            }


        }
    }
}