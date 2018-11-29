using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using TonberryLib;
using System.Configuration;
using Dapper;
using System.Web.Security;

namespace Application.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}