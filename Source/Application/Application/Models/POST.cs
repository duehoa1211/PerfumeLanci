using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Models
{
    public class POST
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng điền tên sản phẩm")]
        [Display(Name = "Tên")]
        public string TITLE { get; set; }
        [Required(ErrorMessage ="Vui lòng điền mô tả sản phẩm")]
        [Display(Name = "Mô tả")]
        [AllowHtml]
        public string CONTENT { get; set; }
        [Required(ErrorMessage ="Vui lòng chọn ảnh đại diện")]
        [Display(Name = "Ảnh đại diện")]
        public string AVARTAR { get; set; }
        [Display(Name = "Ghi chú")]
        public string OPTIONAL { get; set; }
        [Display(Name = "Active")]
        public bool ACTIVE { get; set; }
        [Display(Name = "Loại")]
        public int? ID_TYPE { get; set; }
        [Display(Name = "Nhóm")]
        public int? CATE_ID { get; set; }
        [Display(Name = "Giá")]
        [DataType(DataType.Currency)]
        public decimal? PRICE { get; set; }

        public string SEOURL { get; set; }

        public POST_TYPE Type { get; set; }

        public POST_CATE Category { get; set; }

    }

}