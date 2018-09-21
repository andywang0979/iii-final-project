using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using pet_box.Models;

namespace pet_box.Security {
    public class PasswordSecurity {

        public static string HashSHA1(string value) {
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(value);
            byte[] hash = sha1.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static int GetUserIdByUsernameAndHashedSaltedPassword(string userName, string password) {

            // the default customer ID is 1
            //int customerId = 1;

            PetBoxEntities1 dbForHashCodeCheck = new PetBoxEntities1();

            Customer userNameQuery = (from o in dbForHashCodeCheck.Customers
                                      where o.CustomerLoginName == userName
                                      select o).SingleOrDefault();

            if (userNameQuery == null) {
                return 1;
            }

            string toBeTestedPassword = PasswordSecurity.HashSHA1(password + userNameQuery.CustomerGuid);

                if (toBeTestedPassword == userNameQuery.CustomerPassword) {
                    return userNameQuery.CustomerID;
                }

            return 1;
        }
    }
}