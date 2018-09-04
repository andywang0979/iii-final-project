using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pet_box.Models;
using pet_box.ViewModels;


namespace pet_box.Controllers
{
    public class OptionalItemController : Controller
    {
        PetBoxEntities3 db = new PetBoxEntities3();
        // GET: OptionalItem
        public ActionResult Index()
        {
            OptionalItemViewModel viewM = new OptionalItemViewModel();

            // create a diction to contain the variable name and the object
            Dictionary<string, CategoryProductModel> dummy = new Dictionary<string, CategoryProductModel>();
            viewM.CategoryProductModelDic = dummy;
            
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

            return View(viewM);
        }
    }
}