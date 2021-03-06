﻿using Application.Models;
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
    //[CompressContent]
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
                frontPage.Intro = connection.QueryFirstOrDefault<OPTIONAL>(string.Format(@"
                          SELECT [ID]
                              ,[NAME]
                              ,[CONTENTS]
                          FROM [dbo].[Optional] 
                          WHERE [ID] = 1
                "));
                frontPage.Sliders = connection.Query<SLIDER, POST, SLIDER>(string.Format(@"
                          SELECT SLIDER.[ID],SLIDER.[NAME],SLIDER.[URI],SLIDER.[IMAGE],
                            	 PRODUCT.[ID],PRODUCT.[TITLE],PRODUCT.[CONTENT],PRODUCT.[AVARTAR],PRODUCT.[OPTIONAL],PRODUCT.[ACTIVE],PRODUCT.[ID_TYPE],PRODUCT.[CATE_ID],PRODUCT.[PRICE],PRODUCT.[SEOURL]
                          FROM [dbo].[SLIDER] SLIDER
                                INNER JOIN [dbo].[POST] PRODUCT ON CAST(SLIDER.[URI] AS int) = PRODUCT.[ID]

                "), (Slider, Post) =>
                {
                    Slider.SanPham = Post ?? new POST();
                    return Slider;
                }, splitOn: "ID");
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
                return View(modules.FirstOrDefault(z => z.ID == id));
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}