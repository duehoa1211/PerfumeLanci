using Application.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class HomeController : Controller
    {
        IDbConnection connection;
        public HomeController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
        [HttpGet]
        public ActionResult Index()
        {
            using (connection)
            {
                FrontPage frontPage = new FrontPage();
                frontPage.Posts = connection.Query<POST>(string.Format(@" 
                           SELECT Post.[ID], Post.[TITLE], Post.[CONTENT], Post.[AVARTAR], Post.[ID_TYPE], Post.[CATE_ID], Post.[PRICE], Post.[SEOURL], Post.[ACTIVE]
                           FROM[dbo].[POST] Post
                           WHERE Post.[ACTIVE] = 'True'")).OrderByDescending(z => z.ID).Take(2);
                frontPage.Youtube = connection.Query<URL>(string.Format(@"SELECT [ID]
                                    ,[NAME]
                                    ,[DESCRIP]
                              FROM [dbo].[URL]
                              WHERE [ID] = 1")).FirstOrDefault();
                return View(frontPage);
            }
        }

        [HttpGet]
        public ActionResult Product(int id)
        {
            using (connection)
            {
                var modules = connection.Query<POST>(string.Format(@"
                           SELECT Post.[ID], Post.[TITLE], Post.[CONTENT], Post.[AVARTAR], Post.[ID_TYPE], Post.[CATE_ID], Post.[PRICE], Post.[SEOURL], Post.[ACTIVE]
                           FROM [dbo].[POST] Post
                           WHERE Post.[ACTIVE] = 'True'
                "));
                return View(modules.FirstOrDefault());
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}