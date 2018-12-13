using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Models
{
    public class CART_DETAIL
    {
        public int BILL_ID { get; set; }

        [Required]
        [Display(Name ="Sản phẩm")]
        public int? PRODUCT_ID { get; set; }

        [Required]
        [Display(Name ="Số lượng")]
        public int? QUANTITY { get; set; }

        public POST Product { get; set; }

        public SelectList ProductList { get; set; }
    }
}