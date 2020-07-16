using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniShopWebNHT.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System.Security.Claims;

namespace MiniShopWebNHT.Controllers
{
    public class TaiKhoanController : Controller
    {
        MiniShopEntities db = new MiniShopEntities();
        // GET: TaiKhoan
        public ActionResult DangNhap()
        {
            return View();
        }
        //Google//
        public void SignIn(string ReturnUrl = "/", string type = "")
        {
            if (!Request.IsAuthenticated)
            {
                if (type == "Google")
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "TaiKhoan/GoogleLoginCallback" }, "Google");
                }
            }
        }
        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Redirect("~/");
        }

        [AllowAnonymous]
        public ActionResult GoogleLoginCallback()
        {
            var claimsPrincipal = HttpContext.User.Identity as ClaimsIdentity;

            var loginInfo = SSO.GetLoginInfo(claimsPrincipal);
            if (loginInfo == null)
            {
                return RedirectToAction("DangNhap");
            }
            MiniShopEntities db = new MiniShopEntities(); //DbContext
            var user = db.KhachHangs.FirstOrDefault(x => x.Email == loginInfo.emailaddress);

            if (user == null)
            {
                user = new KhachHang
                {

                    Email = loginInfo.emailaddress,
                    HoTenKH = loginInfo.name,
                    DiaChiKH = loginInfo.nameidentifier,

                };
                db.KhachHangs.Add(user);
                db.SaveChanges();
            }
            Session["username"] = loginInfo.name;
            Session["makh"] = user.MaKH;

            var ident = new ClaimsIdentity(
                    new[] { 
									// adding following 2 claim just for supporting default antiforgery provider
									new Claim(ClaimTypes.NameIdentifier, user.Email),
                                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                                    new Claim(ClaimTypes.Name, user.HoTenKH),
                                    new Claim(ClaimTypes.Email, user.Email),
									// optionally you could add roles if any
									new Claim(ClaimTypes.Role, "User")
                    },
                    CookieAuthenticationDefaults.AuthenticationType);


            HttpContext.GetOwinContext().Authentication.SignIn(
                        new AuthenticationProperties { IsPersistent = false }, ident);
            return Redirect("~/");
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection frmDN)
        {
            
            string sTaiKhoan = frmDN["TenDN"];
            string sMatKhau = GetMD5(frmDN["MatKhau"]);
            //Lấy username và password ở bản ghi đầu tiên
            var user = db.KhachHangs.Where(x => x.TenDN == sTaiKhoan && x.MatKhau == sMatKhau).FirstOrDefault();
            if (user == null)
            {

                ViewBag.error = "Tên đăng nhập hoặc mật khẩu sai";
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Đăng nhập không thành công!');
                         window.location.href='/TaiKhoan/DangNhap'
                         </script>
                      ");
            }
            else
            {
                //ViewBag.avatar = user.Avatar;
                //ViewBag.Online = user.IsActive;
                //Session["Online"] = user.IsActive;
                //Session["Avatar"] = user.Avatar;
                Session["MaKH"] = user.MaKH;
                Session["Email"] = user.TenDN;
                Session["Password"] = user.MatKhau;
                //return View(user)

                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Đăng nhập thành công!');
                         window.location.href='/Home/TrangChu'
                         </script>
                      ");
            }
        }
        public string GetMD5(string MD5)
        {
            string str = "";
            byte[] A = System.Text.Encoding.UTF8.GetBytes(MD5);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            A = md5.ComputeHash(A);
            foreach (Byte b in A)
            {
                str += b.ToString("X2");
            }
            return str;
        }

        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection frmDK, KhachHang KH)
        {
            KH.HoTenKH = frmDK["hoten"];
            KH.DiaChiKH = frmDK["diachi"];
            KH.TenDN = frmDK["tendn"];
            KH.MatKhau = GetMD5(frmDK["matkhau"]);
            KH.Email = frmDK["email"];
            db.KhachHangs.Add(KH);
            db.SaveChanges();
            return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Đăng Ký thành công!');
                         window.location.href='/Home/TrangChu'
                         </script>
                      ");
        }

        public ActionResult DangXuat()
        {

            Session["GioHang"] = null;
            Session["MaKH"] = null;
            Session["Email"] = null;
            Session["Password"] = null;
            return RedirectToAction("TrangChu", "Home");
            
        }
    }
}