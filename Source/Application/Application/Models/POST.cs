using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Models
{
    public class POST
    {
        public int ID { get; set; }

        public string TITLE { get; set; }

        public string CONTENT { get; set; }

        public string AVARTAR { get; set; }

        public string OPTIONAL { get; set; }

        public bool? ACTIVE { get; set; }

        public int? ID_TYPE { get; set; }

        public int? CATE_ID { get; set; }

        public decimal? PRICE { get; set; }

        public string SEOURL { get; set; }

        public POST_TYPE Type { get; set; }

        public POST_CATE Category { get; set; }

    }

}