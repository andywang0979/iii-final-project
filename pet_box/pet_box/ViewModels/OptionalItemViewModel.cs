using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pet_box.Models;

namespace pet_box.ViewModels {
    public class OptionalItemViewModel {

        public IEnumerable<Product> Products { get; set; }

        public Dictionary<string, CategoryProductModel> CategoryProductModelDic { get; set; }

    }
}