using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Models
{
    public class CART_DETAIL
    {
        public int BILL_ID { get; set; }

        public int? PRODUCT_ID { get; set; }

        public int? QUANTITY { get; set; }

        public POST Product { get; set; }

    }
}