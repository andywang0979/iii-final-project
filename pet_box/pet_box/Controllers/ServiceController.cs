using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pet_box.Models;

namespace pet_box.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Service

        PetBoxEntities1 db = new PetBoxEntities1();

        public ActionResult Management()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Management(string EmployeeLoginName, string EmployeePassword)
        {
            var Employee = db.Employees
                .Where(c => c.EmployeeLoginName == EmployeeLoginName && c.EmployeePassword == EmployeePassword)
                .FirstOrDefault();

            if (Employee == null)
            {
                ViewBag.Message = "帳密錯誤 登入失敗";
                return View();
            }


            Session["Customer"] = Employee;

            return RedirectToAction("Workdistinction");
        }

        public ActionResult Workdistinction()
        {
            return View();
        }

        public ActionResult Customerservice()
        {
            return View();
        }

        public ActionResult Customerservice2()
        {
            return View();
        }




    }
}