using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pet_box.Models {
    public class ShoppingCartObjectModel {
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        // this quantity is the number of same item that the customer want to buy
        public int Quantity { get; set; }
        public float Discount { get; set; }
        public string ImgLocation { get; set; }
        public decimal UnitPrice { get; set; }
    }
}