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
using System.Data;
using Application.Models;

namespace Application.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IDbConnection connection;
        public AdminController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        #region PostCate
        [HttpGet]
        public ActionResult PostCate()
        {
            using (connection)
            {
                var models = connection.Query<POST_CATE>(string.Format(@"
                                        SELECT [ID]
                                              ,[CATE_NAME]
                                              ,[DESCRIP]
                                              ,[THUMBNAIL]
                                          FROM [dbo].[POST_CATE]
                "));
                return View(models);
            }
        }
        #region Insert
        [HttpGet]
        public ActionResult PostCateInsertView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PostCateInsert(POST_CATE model)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                    INSERT INTO [dbo].[POST_CATE]
                               ([CATE_NAME]
                               ,[DESCRIP]
                               ,[THUMBNAIL])
                         VALUES
                               (N'{0}'
                               ,N'{1}'
                               ,N'{2}')

                    ", model.CATE_NAME, model.DESCRIP, model.THUMBNAIL));
                    return RedirectToAction("PostCate");
                }
                return View("PostCateInsertView", model);
            }
        }
        #endregion
        #region Update
        [HttpGet]
        public ActionResult PostCateUpdateView(int id)
        {
            using (connection)
            {
                var model = connection.Query<POST_CATE>(string.Format(@"
                            SELECT [ID]
                                ,[CATE_NAME]
                                ,[DESCRIP]
                                ,[THUMBNAIL]
                            FROM [dbo].[POST_CATE]
                            WHERE [ID] = {0}
                            ", id)).FirstOrDefault();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostCateUpdate(POST_CATE model)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                            UPDATE [dbo].[POST_CATE]
                               SET [CATE_NAME] = N'{1}'
                                  ,[DESCRIP] = N'{2}'
                                  ,[THUMBNAIL] = N'{3}'
                             WHERE [ID] = {0}
                            ", model.ID, model.CATE_NAME,
                               model.DESCRIP, model.THUMBNAIL));
                    return RedirectToAction("PostCate");
                }
                return View("PostCateUpdateView", model);
            }
        }
        #endregion
        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostCateDelete(int cateID)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                        DELETE FROM [dbo].[POST_CATE]
                        WHERE [ID] = {0}"
                          , cateID));
                    return RedirectToAction("PostCate");
                }
                return RedirectToAction("PostCate");
            }
        }
        #endregion
        #endregion

        #region PostType
        [HttpGet]
        public ActionResult PostType()
        {
            using (connection)
            {
                var models = connection.Query<POST_TYPE>(string.Format(@"
                               SELECT [ID]
                                   ,[NAME_TYPE]
                                   ,[DESCRIP]
                                   ,[THUMBNAIL]
                                   ,[ID_CATE]
                               FROM [dbo].[POST_TYPE]
                "));
                return View(models);
            }
        }
        #region Insert
        [HttpGet]
        public ActionResult PostTypeInsertView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PostTypeInsert(POST_TYPE model)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                    INSERT INTO [dbo].[POST_TYPE]
                               ([NAME_TYPE]
                               ,[DESCRIP]
                               ,[THUMBNAIL])
                         VALUES
                               (N'{0}'
                               ,N'{1}'
                               ,N'{2}')

                    ", model.NAME_TYPE, model.DESCRIP, model.THUMBNAIL));
                    return RedirectToAction("PostType");
                }
                return View("PostTypeInsertView", model);
            }
        }
        #endregion
        #region Update
        [HttpGet]
        public ActionResult PostTypeUpdateView(int id)
        {
            using (connection)
            {
                var model = connection.Query<POST_TYPE>(string.Format(@"
                            SELECT [ID]
                                ,[NAME_TYPE]
                                ,[DESCRIP]
                                ,[THUMBNAIL]
                            FROM [dbo].[POST_TYPE]
                            WHERE [ID] = {0}
                            ", id)).FirstOrDefault();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostTypeUpdate(POST_TYPE model)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                           UPDATE [dbo].[POST_TYPE]
                           SET [NAME_TYPE] = N'{1}' 
                              ,[DESCRIP] = N'{2}'
                              ,[THUMBNAIL] = N'{3}'
                            WHERE [ID] = {0}
                            ", model.ID, model.NAME_TYPE,
                               model.DESCRIP, model.THUMBNAIL));
                    return RedirectToAction("PostType");
                }
                return View("PostTypeUpdateView", model);
            }
        }
        #endregion
        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostTypeDelete(int typeID)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                        DELETE FROM [dbo].[POST_TYPE]
                        WHERE [ID] = {0}"
                          , typeID));
                    return RedirectToAction("PostType");
                }
                return RedirectToAction("PostType");
            }
        }
        #endregion
        #endregion
    }
}