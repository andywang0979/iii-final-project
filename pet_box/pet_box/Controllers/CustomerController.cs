using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pet_box.Models;

namespace pet_box.Controllers
{
    public class CustomerController : Controller
    {
        
        PetBoxEntities1 db = new PetBoxEntities1();
        public ActionResult Index()
        {
            if (Session["Customer"] == null)
            {
                return View("Index", "_Layout");
            }
            return View("Index", "_Layout2");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string CustomerEmail, string CustomerPassword)
        {
            var Customer = db.Customers
                .Where(c => c.CustomerEmail == CustomerEmail && c.CustomerPassword == CustomerPassword)
                .FirstOrDefault();

            if (Customer == null)
            {
                ViewBag.Message = "帳密錯誤 登入失敗";
                return View();
            }

            Session["Welcome"] = Customer.CustomerName + " " + "歡迎光臨";
            Session["Customer"] = Customer;
            Session["CustomerID"] = Customer.CustomerID;

            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear(); 
            return RedirectToAction("Index");
        }

        public ActionResult Index2()
        {
            
            if (Session["Customer"] == null)
            {
                return View("Index2", "_Layout");
            }
            return View("Index2", "_Layout2");
        }

        public ActionResult Index3()
        {
           
            if (Session["Customer"] == null)
            {
                return View("Index3", "_Layout");
            }
            return View("Index3", "_Layout2");
        }



        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer cus)
        {
            if(string.IsNullOrEmpty(cus.CustomerEmail))
            {
                ViewBag.Message2 = "請填寫信箱";
                return View("Register", cus);
            }
            if (string.IsNullOrEmpty(cus.CustomerPassword))
            {
                ViewBag.Message3 = "請填寫密碼";
                return View("Register", cus);
            }

            var Customer = db.Customers
               .Where(c => c.CustomerEmail == cus.CustomerEmail)
               .FirstOrDefault();

            if (Customer == null)
            {
                db.Customers.Add(cus);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號己有人使用，註冊失敗";
            return View();
        }

    }
}