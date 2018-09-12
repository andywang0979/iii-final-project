using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pet_box.Models {
    public class PartialListItemModel {
        public int PartialListProductId { get; set; }
        public string PartialListProductName { get; set; }
        public string PartialListProductImgLocation { get; set; }
        public int PartialListProductBuyQuantity { get; set; }
        public decimal PartialListProductUnitPrice { get; set; }
    }
}