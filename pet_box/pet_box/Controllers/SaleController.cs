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
        public ActionResult AddItem(Product pro)
        {
            if (Request["OkOrCancel"] == "Cancel")
            {
                return RedirectToAction("Shelf");
            }

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
        public ActionResult EditItem(Product pro)
        {
            if (Request["OkOrCancel"] == "Cancel")
            {
                return RedirectToAction("Shelf");
            }

            db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Shelf");
        }


    }
}