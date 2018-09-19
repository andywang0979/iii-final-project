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
            var query = from o in db.Opinions
                        select o;
            List<Opinion> result = query.ToList();
            return View(result);
        }

        public ActionResult Customerservice2(int? id)
        {

            Opinion op = db.Opinions.Find(id);
            if (op == null)
            {
                return RedirectToAction("Customerservice");
            }

            return View(op);
        }

        [HttpPost]
        public ActionResult Customerservice2(Opinion op)
        {
            
            op.OpinionFeedbackTime = DateTime.Now.ToString("yyyyMMdd HH:mm");
            db.Entry(op).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Customerservice");

        }


    }
}