using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Models
{
    public class FrontPage
    {
        public IEnumerable<POST> Posts { get; set; }
        public URL Youtube { get; set; }
    }
}