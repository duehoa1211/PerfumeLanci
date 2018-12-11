using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Models
{
    public class OPTIONAL
    {
        public int ID { get; set; }

        public string NAME { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập nội dung giới thiệu")]
        [AllowHtml]
        public string CONTENTS { get; set; }

    }
}