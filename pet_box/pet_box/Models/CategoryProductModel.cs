using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pet_box.Models {
    public class CategoryProductModel {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }
}