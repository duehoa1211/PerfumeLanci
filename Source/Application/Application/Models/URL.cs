﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Application.Models
{
    public class URL
    {
        public int ID { get; set; }

        public string NAME { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập link")]
        public string DESCRIP { get; set; }

    }

}