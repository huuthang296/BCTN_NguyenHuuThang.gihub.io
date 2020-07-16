using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniShopWebNHT.Models;

namespace MiniShopWebNHT.Controllers
{
    public class LoginAdminController : Controller
    {
        // GET: LoginAdmin
        public ActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAdmin(ADMIN usermodel)
        {
            //New dbConnect
            using (MiniShopEntities db = new MiniShopEntities())
            {

                //Lấy username và password ở bản ghi đầu tiên
                var user = db.ADMINs.Where(x => x.TenDNAD == usermodel.TenDNAD && x.MatKhauAD == usermodel.MatKhauAD).FirstOrDefault();
                if (user == null)
                {

                    ViewBag.error = "Email or Password is fail";
                    return View("LoginAdmin", usermodel);
                }
                else
                {
                    //ViewBag.avatar = user.Avatar;
                    //ViewBag.Online = user.IsActive;
                    //Session["Online"] =user.IsActive;
                    //Session["Avatar"] = user.Avatar;
                    Session["Email"] = user.EmailAD;
                    Session["Username"] = user.TenDNAD;
                    //return View(user)

                    return RedirectToAction("Index", "AdminCRUD");
                }

            }
        }
        public ActionResult Test()
        {
            return View();
        }
    }
}