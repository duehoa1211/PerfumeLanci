using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Models
{
    public class CART
    {
        public int BILL_ID { get; set; }

        [Display(Name ="Ghi chú")]
        public string INFOS { get; set; }

        [Required(ErrorMessage ="Vui lòng điền họ tên")]
        [Display(Name ="Họ tên")]
        public string CUSTOMER { get; set; }

        [Required(ErrorMessage ="Vui lòng điền địa chỉ")]
        [Display(Name ="Địa chỉ")]
        public string ADDRESS { get; set; }

        [Required(ErrorMessage ="Vui lòng nhập số điện thoại")]
        [Display(Name ="Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public string PHONENUMBER { get; set; }

        [Display(Name ="Email")]
        public string EMAIL { get; set; }

        public List<CART_DETAIL> CartDetail { get; set; }

    }
}