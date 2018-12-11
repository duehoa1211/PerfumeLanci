using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Models
{
    public class SLIDER
    {
        public int ID { get; set; }

        [AllowHtml]
        [Display(Name ="Nội dung")]
        public string NAME { get; set; }
        [Display(Name ="Sản phẩm")]
        public string URI { get; set; }

        [Required(ErrorMessage ="Vui lòng chọn ảnh")]
        [Display(Name ="Ảnh")]
        public string IMAGE { get; set; }

        public POST SanPham { get; set; }

        public SelectList ListSanPham { get; set; }
    }

}