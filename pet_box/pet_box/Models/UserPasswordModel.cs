using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pet_box.Models {
    public class UserPasswordModel {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public Guid UserGuid { get; set; }

    }


}