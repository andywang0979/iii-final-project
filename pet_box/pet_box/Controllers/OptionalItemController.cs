using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pet_box.Models;
using pet_box.ViewModels;


namespace pet_box.Controllers {
    public class OptionalItemController : Controller {

        PetBoxEntities1 db = new PetBoxEntities1();
        // GET: OptionalItem

        public ActionResult Index() {
            if (TempData["itemList"] != null) {
                //return Content("some some in temp data itemlist");
                TempData.Keep("itemList");
            }

            OptionalItemViewModel viewM = new OptionalItemViewModel();


            Dictionary<string, CategoryProductModel> dummy = new Dictionary<string, CategoryProductModel>();
            viewM.CategoryProductModelDic = dummy;
            // loop test
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

        [HttpPost]
        public ActionResult Index(int? id) {

            if (Request.Form["okOrCancel"] == "ok") {

                // a string: 2,5,7,8
                // each number is ProductID
                string productIdstring = Request.Form["boxItem"];

                List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();

                if (string.IsNullOrEmpty(productIdstring)) {
                    productIdstring = "";
                    // to ShoppingCart to check again
                    // or to a view show you buy nothing.
                    TempData.Keep("itemList");
                    return RedirectToAction("ShoppingCart");
                }


                // add petbox
                productIdstring += ",2";

                string[] productIdStringArray = productIdstring.Split(',');

                // translate from an array of string to an array of int
                int[] productIdIntArray = productIdStringArray
                    .Select(s => { int i; return int.TryParse(s, out i) ? i : (int?)null; })
                    .Where(i => i.HasValue)
                    .Select(i => i.Value)
                    .ToArray();


                // user choose some items, and no itemList exist yet

                if (TempData["itemList"] == null) {

                    foreach (var item in productIdIntArray) {
                        ShoppingCartObjectModel instance = new ShoppingCartObjectModel();

                        // here instance.CustomerID should be equal to Session[CustomerID]
                        instance.CustomerID = Convert.ToInt32(Session["CustomerID"]);
                        instance.ProductID = item;
                        instance.Quantity = 1;

                        itemObjList.Add(instance);
                    }
                }
                // if there is item in itemList
                else {
                    itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;

                    foreach (int itemId in productIdIntArray) {
                        ShoppingCartObjectModel tempObj = itemObjList.Find(obj => itemId == obj.ProductID);

                        if (tempObj != null) {

                            tempObj.Quantity++;

                        } else {

                            ShoppingCartObjectModel addObj = new ShoppingCartObjectModel();
                            addObj.CustomerID = Convert.ToInt32(Session["CustomerID"]);
                            addObj.ProductID = itemId;
                            addObj.Quantity = 1;
                            itemObjList.Add(addObj);
                        }
                    }

                }

                TempData["itemList"] = itemObjList;
                TempData.Keep("itemList");

                return RedirectToAction("ShoppingCart");

            }

            if (TempData["shoppingURL"] != null) {
                string returnPath = TempData["shoppingURL"].ToString();
                string[] pathArray = returnPath.Split('/');

                return RedirectToAction(pathArray[1], pathArray[0]);
            } else {
                return RedirectToAction("", "Customer");
            }


            // back to index of shopping
            //return RedirectToAction("", "");

        }


        /// <summary>
        /// use tempData
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult ShoppingCart() {

            if (TempData["itemList"] == null) {
                return RedirectToAction("", "Customer");
            }

            List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();
            itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;
            TempData.Keep("itemList");

            if (itemObjList.Count() == 0) {
                return RedirectToAction("", "Customer");
            }


            List<int> productIdList = new List<int>();

            foreach (var item in itemObjList) {
                productIdList.Add(item.ProductID);
            }

            var queryProduct = db.Products.Where(x => productIdList.Contains(x.ProductID));
            List<Product> productList = queryProduct.ToList();

            var partialList = from p in productList
                              join q in itemObjList
                              on p.ProductID equals q.ProductID
                              select new PartialListItemModel() {
                                  PartialListProductId = p.ProductID,
                                  PartialListProductName = p.ProductName,
                                  PartialListProductImgLocation = p.ProductImageLocation,
                                  PartialListProductBuyQuantity = q.Quantity,
                                  PartialListProductUnitPrice = p.ProductPrice
                              };

            List<PartialListItemModel> partialModelList = partialList.ToList();

            PartialListModel partialObjList = new PartialListModel();
            partialObjList.PartialList = partialModelList;

            return View(partialObjList);

        }


        [HttpPost]
        public ActionResult ShoppingCart(int? id, PartialListModel partialModelList) {

            List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();
            itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;
            TempData.Keep("itemList");

            if (Request.Form["okOrCancel"] == "ok") {

                // copy the quantity from the model posted from delete button, 
                // to the itemList, then save the new quantity to tempData.
                foreach (ShoppingCartObjectModel item in itemObjList) {
                    PartialListItemModel itemToCopy = partialModelList.PartialList.SingleOrDefault(r => r.PartialListProductId == item.ProductID);
                    item.Quantity = itemToCopy.PartialListProductBuyQuantity;
                    item.UnitPrice = itemToCopy.PartialListProductUnitPrice;
                    item.ProductName = itemToCopy.PartialListProductName;
                }

                TempData["itemList"] = itemObjList;
                TempData.Keep("itemList");

                return RedirectToAction("DeliveryInfo");
            } else if (Request.Form["okOrCancel"] == "continue") {

                if (TempData["shoppingURL"] != null) {

                    // then there could be 3 cases: "/", "/Home", "/Home/Index"
                    string returnPath = TempData["shoppingURL"].ToString();
                    //return Content(returnPath);
                    string[] pathArray = returnPath.Split('/');
                    if (pathArray.Length == 3) {
                        return RedirectToAction(pathArray[2], pathArray[1]);
                    } else if (pathArray.Length == 2 && pathArray[1] != "") {
                        return RedirectToAction(pathArray[1], "");
                    } else {
                        return RedirectToAction("", "");
                    }

                } else {
                    return RedirectToAction("", "");
                }

            }
            return RedirectToAction("", "");
        }

        public ActionResult DeliveryInfo() {
            //show member contact info, user can also modify here.
            int customerIdInt = Convert.ToInt32(Session["CustomerID"]);

            // if the client is not a member. and do not want to be a member.
            List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();
            itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;
            TempData.Keep("itemList");

            // user use broser's previsous page from deilvery page or other situation
            if (TempData["itemList"] == null) {
                return RedirectToAction("", "Customer");
            }

            DeliveryInfoViewModel viewM = new DeliveryInfoViewModel();
            viewM.itemList = itemObjList;


            // assign unit price of each item
            foreach (ShoppingCartObjectModel item in itemObjList) {
                Product queryProduct = (from o in db.Products
                                        where o.ProductID == item.ProductID
                                        select o).SingleOrDefault();
                item.UnitPrice = queryProduct.ProductPrice;
            }


            //calculate totalPrice
            viewM.PriceTotal = 0;
            foreach (ShoppingCartObjectModel item in itemObjList) {
                viewM.PriceTotal += item.UnitPrice * item.Quantity;
            }


            // here should check if the member login in, 
            // temporary solution: here just use default value of `int` if no assignment,
            // 
            if (customerIdInt == 1) {
                Customer newCustomer = new Customer();
                viewM.customer = newCustomer;
                return View(viewM);
            }


            //int customerId = itemObjList[0].CustomerID;
            var query = from o in db.Customers
                        where o.CustomerID == customerIdInt
                        select o;

            viewM.customer = query.SingleOrDefault();

            return View(viewM);

        }

        [HttpPost]
        public ActionResult DeliveryInfo(DeliveryInfoViewModel divm) {

            // check the list passed
            List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();
            itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;
            TempData.Keep("itemList");

            int customerIdInt = Convert.ToInt32(Session["CustomerID"]);

            if (Request.Form["okOrCancel"] == "ok") {



                // update Customers
                // 1 is non-member. 
                //if (itemObjList[0].CustomerID != 1) {
                if (customerIdInt != 1) {
                    //int myInt = Convert.ToInt32(Session["CustomerID"]);

                    var customerDatabase = db.Customers
                        .Where(x => x.CustomerID == customerIdInt)
                        .FirstOrDefault();
                    customerDatabase.CustomerAddress = divm.customer.CustomerAddress;
                    customerDatabase.CustomerEmail = divm.customer.CustomerEmail;
                    customerDatabase.CustomerMobilePhone = divm.customer.CustomerMobilePhone;
                    customerDatabase.CustomerName = divm.customer.CustomerName;

                } else {
                    divm.customer.CustomerRole = 2;
                    db.Customers.Add(divm.customer);
                }


                // create Order
                Order newOrder = new Order();

                newOrder.CustomerID = customerIdInt;
                newOrder.OrderDateTime = DateTime.Now.ToString("yyyyMMdd HH:mm");
                newOrder.OrderShipAddress = divm.customer.CustomerAddress;
                db.Orders.Add(newOrder);

                // create OrderDetails
                foreach (ShoppingCartObjectModel item in itemObjList) {
                    OrderDetail newOrderDetail = new OrderDetail();
                    newOrderDetail.OrderID = newOrder.OrderID;
                    newOrderDetail.ProductID = item.ProductID;
                    newOrderDetail.Quantity = item.Quantity;
                    newOrderDetail.Discount = item.Discount;
                    newOrderDetail.UnitPrice = item.UnitPrice;
                    db.OrderDetails.Add(newOrderDetail);
                }


                db.SaveChanges();

                // decrease quanty in table Products
                for (int i = 0; i < itemObjList.Count(); i++) {
                    int itemId = itemObjList[i].ProductID;
                    int itemQuantity = itemObjList[i].Quantity;
                    var productDatabase = db.Products
                        .Where(x => x.ProductID == itemId)
                        .FirstOrDefault();
                    productDatabase.ProductQuantity = productDatabase.ProductQuantity - itemQuantity;
                }

                db.SaveChanges();

                return RedirectToAction("OrderSuccess");

            }



            return RedirectToAction("ShoppingCart");
        }



        public ActionResult OrderSuccess(Customer customerFromForm) {
            // clear itemList, just to make sure it is empty
            //List<ShoppingCartObjectModel> emptyList = new List<ShoppingCartObjectModel>();
            TempData["itemList"] = null;

            return View();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> ProductID</param>
        /// <returns></returns>
        // CartItemDelete
        [HttpPost]
        public ActionResult CartItemDelete(int? id, PartialListModel partialModelList) {
            List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();
            itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;
            TempData.Keep("itemList");

            // remove from itemList, or say itemObjList
            ShoppingCartObjectModel itemToRemove = itemObjList.SingleOrDefault(r => r.ProductID == id);

            if (itemToRemove != null) {
                itemObjList.Remove(itemToRemove);
            }

            PartialListItemModel itemToRemoveFromPartialModel = partialModelList.PartialList.SingleOrDefault(r => r.PartialListProductId == id);
            if (itemToRemoveFromPartialModel != null) {
                partialModelList.PartialList.Remove(itemToRemoveFromPartialModel);
            }

            // copy the quantity from the model posted from delete button, 
            // to the itemList, then save the new quantity to tempData.
            foreach (ShoppingCartObjectModel item in itemObjList) {
                PartialListItemModel itemToCopy = partialModelList.PartialList.SingleOrDefault(r => r.PartialListProductId == item.ProductID);
                item.Quantity = itemToCopy.PartialListProductBuyQuantity;
            }



            TempData["itemList"] = itemObjList;
            TempData.Keep("itemList");
            return RedirectToAction("ShoppingCart");

        }


        [HttpPost]
        public ActionResult SingleBuy(int id) {
            List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();


            if (TempData["itemList"] == null) {

                ShoppingCartObjectModel newItem = new ShoppingCartObjectModel();
                // here should be the logined customer's id
                newItem.CustomerID = Convert.ToInt32(Session["CustomerID"]);
                newItem.ProductID = id;
                newItem.Quantity = Convert.ToInt32(Request.Form["buyQuantity"]);
                itemObjList.Add(newItem);


            } else {

                itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;

                ShoppingCartObjectModel tempObj = itemObjList.Find(obj => id == obj.ProductID);

                if (tempObj != null) {

                    tempObj.Quantity += Convert.ToInt32(Request.Form["buyQuantity"]);

                } else {

                    ShoppingCartObjectModel addObj = new ShoppingCartObjectModel();
                    addObj.CustomerID = Convert.ToInt32(Session["CustomerID"]);
                    addObj.ProductID = id;
                    addObj.Quantity = Convert.ToInt32(Request.Form["buyQuantity"]);

                    //shopCartItemList.shopCartObjList.Add(instance);
                    itemObjList.Add(addObj);
                }

            }

            TempData["itemList"] = itemObjList;
            TempData.Keep("itemList");

            return RedirectToAction("ShoppingCart", "OptionalItem");
        }


        public ActionResult CategoryItem(int? id) {

            TempData["shoppingURL"] = Request.Url.PathAndQuery;

            if (TempData["itemList"] != null) {
                //return Content("some some in temp data itemlist");
                TempData.Keep("itemList");
            }

            CategoryItemViewModel viewM = new CategoryItemViewModel();
            viewM.ProductList = new List<CategoryItemModel>();

            var queryProduct = (from o in db.Products
                                where o.ProductID > 1 && o.CategoryID == id
                                select o).ToList();

            foreach (Product item in queryProduct) {
                CategoryItemModel tempInstance = new CategoryItemModel();
                tempInstance.ImgLocation = item.ProductImageLocation;
                tempInstance.ProductID = item.ProductID;
                tempInstance.ProductName = item.ProductName;
                tempInstance.UnitPrice = item.ProductPrice;
                tempInstance.ProductDescription = item.ProductDescription;
                tempInstance.QuantityInStock = (int)item.ProductQuantity;

                viewM.ProductList.Add(tempInstance);
            }


            return View(viewM);


        }


        [HttpPost]
        public ActionResult SingleBuyJavaScript(int id) {
            List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();


            if (TempData["itemList"] == null) {

                ShoppingCartObjectModel newItem = new ShoppingCartObjectModel();
                // here should be the logined customer's id
                newItem.CustomerID = Convert.ToInt32(Session["CustomerID"]);

                newItem.ProductID = id;
                newItem.Quantity = Convert.ToInt32(Request.Form["buyQuantity"]);
                itemObjList.Add(newItem);


            } else {

                itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;

                ShoppingCartObjectModel tempObj = itemObjList.Find(obj => id == obj.ProductID);

                if (tempObj != null) {

                    tempObj.Quantity += Convert.ToInt32(Request.Form["buyQuantity"]);

                } else {

                    ShoppingCartObjectModel addObj = new ShoppingCartObjectModel();
                    addObj.CustomerID = Convert.ToInt32(Session["CustomerID"]);
                    addObj.ProductID = id;
                    addObj.Quantity = Convert.ToInt32(Request.Form["buyQuantity"]);


                    itemObjList.Add(addObj);
                }

            }

            TempData["itemList"] = itemObjList;
            TempData.Keep("itemList");

            return RedirectToAction("CountShoppingCartItemTotal", "OptionalItem");

        }

        public ActionResult CountShoppingCartItemTotal() {
            if (TempData["itemList"] != null) {
                List<ShoppingCartObjectModel> itemObjList = new List<ShoppingCartObjectModel>();
                itemObjList = TempData["itemList"] as List<ShoppingCartObjectModel>;
                TempData.Keep("itemList");

                int totalQuantity = 0;
                foreach (ShoppingCartObjectModel item in itemObjList) {
                    totalQuantity += item.Quantity;
                }

                return Content(totalQuantity.ToString());
            }

            return Content("0");
        }


        public ActionResult CheckCustomerLogin() {
            if (TempData["itemList"] != null) {
                //return Content("some some in temp data itemlist");
                TempData.Keep("itemList");
            }

            // check if user login, probably use another session or other way.
            //return Content(Session["CustomerID"].ToString());
            return Content(Session["CustomerID"].ToString());
        }

    }
}