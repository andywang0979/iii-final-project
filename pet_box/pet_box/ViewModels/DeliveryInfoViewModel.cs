using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pet_box.Models;

namespace pet_box.ViewModels {
    public class DeliveryInfoViewModel {
        public Customer customer { get; set; }
        

        public List<ShoppingCartObjectModel> itemList { get; set; }
        public IEnumerable<Product> Products { get; set; }

        public decimal PriceTotal { get; set; }
    }
}