using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Application.Models
{
    public class POST_TYPE
    {
        public int ID { get; set; }

        [Required]
        [Display(Name ="Tên Loại")]
        public string NAME_TYPE { get; set; }
        [Display(Name ="Ghi Chú")]
        public string DESCRIP { get; set; }
        [Display(Name ="Avatar")]
        public string THUMBNAIL { get; set; }

        public int? ID_CATE { get; set; }

        public List<POST> Posts { get; set; }

        public POST_CATE Category { get; set; }

    }
}