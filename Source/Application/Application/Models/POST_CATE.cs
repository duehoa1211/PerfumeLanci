using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Application.Models
{
    public class POST_CATE
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng điền tên nhóm")]
        [Display(Name ="Tên Nhóm")]
        public string CATE_NAME { get; set; }

        [Display(Name ="Ghi Chú")]
        public string DESCRIP { get; set; }

        public string THUMBNAIL { get; set; }

        public List<POST> Posts { get; set; }

        public List<POST_TYPE> Types { get; set; }

    }
}