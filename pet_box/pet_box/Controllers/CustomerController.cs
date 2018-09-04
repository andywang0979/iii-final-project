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

        PetBoxEntities3 db = new PetBoxEntities3();
        public ActionResult Index()
        {
            var products = db.Products.ToList();
            if (Session["Customer"] == null)
            {
                return View("Index", "_Layout", products);
            }
            return View("Index", "_Layout2", products);
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
            var products = db.Products.ToList();
            if (Session["Customer"] == null)
            {
                return View("Index2", "_Layout", products);
            }
            return View("Index2", "_Layout2", products);
        }

        public ActionResult Index3()
        {
            var products = db.Products.ToList();
            if (Session["Customer"] == null)
            {
                return View("Index3", "_Layout", products);
            }
            return View("Index3", "_Layout2", products);
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
                ViewBag.Message = "請填寫信箱";
                return View("Register", cus);
            }
            if (string.IsNullOrEmpty(cus.CustomerPassword))
            {
                ViewBag.Message2 = "請填寫密碼";
                return View("Register", cus);
            }

            db.Customers.Add(cus);
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        public ActionResult OrderCompleted()
        {
            if (Session["Customer"] == null)
            {
                return View("Login", "_Layout");
            }
            return View("OrderCompleted", "_Layout2");
        }

    }
}