using ClosedXML.Excel;
using MiniShopWebNHT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniShopWebNHT.Controllers
{
    public class EmployeeController : Controller
    {
        public IList<EmployeeViewModel> GetEmployeeList()
        {
            DbAccessContext db = new DbAccessContext();
            var employeeList = (from e in db.Employees
                                join d in db.Departments on e.DepartmentId equals d.DepartmentId
                                select new EmployeeViewModel
                                {
                                    Name = e.Name,
                                    Email = e.Email,
                                    Age = (int)e.Age,
                                    Address = e.Address,
                                    Department = d.DepartmentName
                                }).ToList();
            return employeeList;
        }
        // GET: Employee
        public ActionResult Index()
        {
            return View(this.GetEmployeeList());
        }
        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            gv.DataSource = this.GetEmployeeList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Index");
        }
    }
}