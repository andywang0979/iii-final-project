using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pet_box.Models;

namespace pet_box.ViewModels {
    public class SingleBuyViewModel {
        public IEnumerable<Product> Products { get; set; }

        public Dictionary<string, CategoryProductModel> CategoryProductModelDic { get; set; }

        public ShoppingCartObjectModel oneItem { get; set; }
    }
}