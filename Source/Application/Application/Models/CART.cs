using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Models
{
    public class CART
    {
        public int BILL_ID { get; set; }

        public string INFOS { get; set; }

        public string CUSTOMER { get; set; }

        public string ADDRESS { get; set; }

        public string PHONENUMBER { get; set; }

        public string EMAIL { get; set; }

        public List<CART_DETAIL> CartDetail { get; set; }

    }
}