using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pet_box.Models {
    public class CategoryItemModel {        
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImgLocation { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public string ProductDescription { get; set; }
    }
}