using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pet_box.Models;
using pet_box.ViewModels;
using pet_box.Security;

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

            return View("Index", "_Layout2", viewM);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string LoginName, string Password)
        {
            //var Customer = db.Customers
            //    .Where(c => c.CustomerLoginName == LoginName && c.CustomerPassword == Password)
            //    .FirstOrDefault();

            int customerHashCheckId = PasswordSecurity.GetUserIdByUsernameAndHashedSaltedPassword(LoginName, Password);

            var Employee = db.Employees
                .Where(e => e.EmployeeLoginName == LoginName && e.EmployeePassword == Password)
                .FirstOrDefault();

            if (customerHashCheckId != 1)
            {
                var Customer = db.Customers.Find(customerHashCheckId);
                //return Content(customerHashCheckId.ToString());
                Session["Welcome"] = Customer.CustomerName + " " + "歡迎光臨";
                Session["Customer"] = Customer;
                Session["CustomerID"] = Customer.CustomerID;
                return RedirectToAction("Index");
            }
            else if (Employee != null)
            {
                Session["Welcome"] = Employee.EmployeeLoginName + " " + "管理員";
                Session["Employee"] = Employee;
                Session["EmployeeID"] = Employee.EmployeeID;
                return RedirectToAction("Workdistinction", "Service");
            }
            else
            {
                ViewBag.Message = "帳密輸入錯誤";
            }

            return View();

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
        [ValidateAntiForgeryToken]
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
                Guid userGuid = Guid.NewGuid();
                cus.CustomerGuid = userGuid;
                cus.CustomerPassword = PasswordSecurity.HashSHA1(cus.CustomerPassword + userGuid);

                db.Customers.Add(cus);
                db.SaveChanges();
                return RedirectToAction("RegisterOk",cus);
            }
            ViewBag.Message = "此帳號己有人使用，註冊失敗";
            return View();
        }

        public ActionResult RegisterOk()
        {
            return View();
        }

        public ActionResult Member()
        {
            int customerID = Convert.ToInt32(Session["CustomerID"]);
            Customer cus = (from o in db.Customers
                            where o.CustomerID == customerID
                            select o).Single();
            //Convert.ToInt32(Session["CustomerID"].ToString())
            return View(cus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Member(Customer cus)
        {
            return View();
        }

        public ActionResult MemberEdit(int? id)
        {
            Customer cus = db.Customers.Find(id);
            //if (cus == null) { return RedirectToAction("Member"); }
            return View(cus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberEdit(Customer cus)
        {
            db.Entry(cus).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Member");
        }

        public ActionResult MemberQA()
        {
            Opinion op = new Opinion();
            return View(op);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberQA(Opinion op)
        {
            op.OpinionDateTime = DateTime.Now.ToString("yyyyMMdd HH:mm");
            db.Opinions.Add(op);
            db.SaveChanges();
            return RedirectToAction("Member");
        }

        public ActionResult Memberselect(int? id)
        {

            var query = from o in db.Opinions
                        where o.CustomerID == id
                        select o;
            List<Opinion> op = query.ToList();


            return View(op);
        }

        public ActionResult MemberSee(int? id)
        {

            Opinion op = db.Opinions.Find(id);

            if (op == null)
            {
                return RedirectToAction("Member");
            }

            return View(op);
        }

        public ActionResult MemberOrderTrack()
        {
            return View();
        }

    }
}