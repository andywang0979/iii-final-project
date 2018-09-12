using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pet_box.Models;
using pet_box.ViewModels;

namespace pet_box.Controllers
{
    public class CustomerController : Controller
    {
        
        PetBoxEntities1 db = new PetBoxEntities1();

        public ActionResult Index()
        {
            
            TempData["shoppingURL"] = Request.Url.PathAndQuery;

            if (TempData["itemList"] != null) {
                TempData.Keep("itemList");
            }


            SingleBuyViewModel viewM = new SingleBuyViewModel();
            
            Dictionary<string, CategoryProductModel> dummy = new Dictionary<string, CategoryProductModel>();
            
            viewM.CategoryProductModelDic = dummy;
            // loop
            int? CategoryIdMax = db.Categories.Max(u => (int?)u.CategoryID);
            for (int i = 2; i <= CategoryIdMax; i++) {
                // categoryID with 2 digit
                string s = String.Format("categoryId{0}", i.ToString("D2"));

                // select coresponding collections of products, and tolist
                var queryProductDynamic = (from o in db.Products
                                           where o.ProductID > 1 && o.CategoryID == i
                                           select o).ToList();

                
                CategoryProductModel tempObj = new CategoryProductModel();

                tempObj.CategoryID = queryProductDynamic[0].CategoryID;
                tempObj.CategoryName = queryProductDynamic[0].Category.CategoryName;
                tempObj.Products = queryProductDynamic;

                viewM.CategoryProductModelDic[s] = tempObj;
            }
           
            
            if (Session["Customer"] == null)
            {
                return View("Index", "_Layout", viewM);
                
            }
            return View("Index", "_Layout2", viewM);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string CustomerLoginName, string CustomerPassword)
        {
            var Customer = db.Customers
                .Where(c => c.CustomerLoginName == CustomerLoginName && c.CustomerPassword == CustomerPassword)
                .FirstOrDefault();

            if (Customer == null)
            {
                ViewBag.Message = "帳密錯誤 登入失敗";
                return View();
            }

            // user is not a member
            // not use yet, because the parameter is string, and the value of 
            // non-member customer is null.
            if (Customer.CustomerRole == 2) {
                ViewBag.Message = "您未註冊為會員";
                return RedirectToAction("Index");
            }



            Session["Welcome"] = Customer.CustomerName + " " + "歡迎光臨";
            Session["Customer"] = Customer;
            Session["CustomerID"] = Customer.CustomerID;

            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            // clear 
            if (TempData["itemList"] != null) {
                
                TempData["itemList"] = null;
            }

            Session["CustomerID"] = 1;

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
            if (string.IsNullOrEmpty(cus.CustomerLoginName))
            {
                ViewBag.Message2 = "請填寫帳號";
                return View("Register", cus);
            }
            if (string.IsNullOrEmpty(cus.CustomerPassword))
            {
                ViewBag.Message3 = "請填寫密碼";
                return View("Register", cus);
            }

            var Customer = db.Customers
               .Where(c => c.CustomerLoginName == cus.CustomerLoginName)
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