//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using ClosedXML.Excel;
//using MiniShopWebNHT.Models;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.IO;

//namespace MiniShopWebNHT.Controllers
//{
//    public class ExportfileController : Controller
//    {
//        public object dbc { get; private set; }

//        // GET: Exportfile

//        public ActionResult Index()
//        {
//            var model = MyDataModel
//                .OrderByDescending(p => p.CreateTime)
//                .ToList();
//            return View(model);
//        }
//        [HttpPost]
//        public ActionResult ExportToExcel()
//        {
//            GridView gv = new GridView();
//            gv.DataSource = dbc.MyDataModel
//                .Where(p => p.Activate == true)
//                .Select(r => new {
//                    Names = r.Name,
//                    Emails = r.Email,
//                    CreateTimes = r.CreateTime
//                })
//                .OrderByDescending(p => p.CreateTimes)
//                .ToList();
//            gv.DataBind();
//            Response.Clear();
//            Response.Buffer = true;
//            //Response.AddHeader("content-disposition",
//            // "attachment;filename=GridViewExport.xls");
//            Response.Charset = "utf-8";
//            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
//            Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls");
//            //Mã hóa chữa sang UTF8
//            Response.ContentEncoding = System.Text.Encoding.UTF8;
//            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());

//            StringWriter sw = new StringWriter();
//            HtmlTextWriter hw = new HtmlTextWriter(sw);

//            for (int i = 0; i < gv.Rows.Count; i++)
//            {
//                //Apply text style to each Row
//                gv.Rows[i].Attributes.Add("class", "textmode");
//            }
//            //Add màu nền cho header của file excel
//            gv.HeaderRow.BackColor = System.Drawing.Color.DarkBlue;
//            //Màu chữ cho header của file excel
//            gv.HeaderStyle.ForeColor = System.Drawing.Color.White;

//            gv.HeaderRow.Cells[0].Text = "Tên";
//            gv.HeaderRow.Cells[1].Text = "Email";
//            gv.HeaderRow.Cells[2].Text = "Ngày tạo";
//            gv.RenderControl(hw);

//            Response.Output.Write(sw.ToString());
//            Response.Flush();
//            Response.End();
//            var model = MyDataModel
//                .OrderByDescending(p => p.CreateTime)
//                .ToList();
//            return View("View", model);
//        }
//    }
//}