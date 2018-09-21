using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pet_box.Models;

namespace pet_box.Controllers
{
    public class SaleController : Controller
    {
        // GET: Sale

        PetBoxEntities1 db = new PetBoxEntities1();


        public ActionResult Shelf()
        {
            var query = from o in db.Products
                        select o;
            List<Product> result = query.ToList();
            return View(result);
        }

        public ActionResult AddItem()
        {
            Product pro = new Product();
            return View(pro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(Product pro)
        {
            if (Request["OkOrCancel"] == "Cancel")
            {
                return RedirectToAction("Shelf");
            }

            System.IO.Directory.CreateDirectory(@"C:\final-project\pet_box\pet_box\product_images\site_product\product");

            var f = Request.Files[0];
            if (f.ContentLength >= 0)
            {
                f.SaveAs(@"C:\final-project\pet_box\pet_box\product_images\site_product\product\" + f.FileName);
            }

            pro.ProductImageLocation = "/product_images/site_product/product/" + f.FileName;


            db.Products.Add(pro);
            db.SaveChanges();
            return RedirectToAction("Shelf");
        }

        public ActionResult DeleteItem(int? id)
        {
            Product pro = db.Products.Find(id);
            if (pro == null || Request["OkOrCancel"] == "Cancel")
            {
                return RedirectToAction("Shelf");
            }

            if (Request["OkOrCancel"] == "Ok")
            {
                db.Products.Remove(pro);
                db.SaveChanges();
                return RedirectToAction("Shelf");
            }
            return View(pro);
        }

        public ActionResult EditItem(int? id)
        {
            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return RedirectToAction("Shelf");
            }

            return View(pro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItem(Product pro, FormCollection frm)
        {
            if (Request["OkOrCancel"] == "Cancel")
            {
                return RedirectToAction("Shelf");
            }

            System.IO.Directory.CreateDirectory(@"C:\final-project\pet_box\pet_box\product_images\site_product\product");

            var f = Request.Files[0];
            if (f.ContentLength >= 0)
            {
                f.SaveAs(@"C:\final-project\pet_box\pet_box\product_images\site_product\product\" + f.FileName);
            }

            pro.ProductImageLocation = "/product_images/site_product/product/" + f.FileName;

            db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Shelf");
        }


    }
}