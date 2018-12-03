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
using System.IO;

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
        [ValidateAntiForgeryToken]
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

        #region Post
        [HttpGet]
        public ActionResult Post()
        {
            using (connection)
            {
                var models = connection.Query<POST, POST_CATE, POST_TYPE, POST>(string.Format(@"
                               SELECT Post.[ID], Post.[TITLE], Post.[CONTENT], Post.[AVARTAR], Post.[ID_TYPE], Post.[CATE_ID], Post.[PRICE], Post.[SEOURL], Post.[ACTIVE],
                               	      Cate.[ID], Cate.[CATE_NAME], Cate.[DESCRIP], Cate.[THUMBNAIL],
                               	      Typ.[ID], Typ.[NAME_TYPE], Typ.[THUMBNAIL],Typ.[ID_CATE]
                               FROM [dbo].[POST] Post
                               	INNER JOIN [dbo].[POST_CATE] Cate ON Post.CATE_ID = Cate.ID
                               	INNER JOIN [dbo].[POST_TYPE] Typ ON  Typ.ID = Post.ID_TYPE
                                "), (Post, Cate, Types) =>
                                {
                                    Post.Category = Cate ?? new POST_CATE();
                                    Post.Type = Types ?? new POST_TYPE();
                                    return Post;
                                }, splitOn: "ID,ID");
                return View(models);
            }
        }
        #region Insert
        [HttpGet]
        public ActionResult PostInsertView()
        {
            using (connection)
            {
                var Cates = connection.Query<POST_CATE>(string.Format(@"
                                          SELECT [ID]
                                              ,[CATE_NAME]
                                              ,[DESCRIP]
                                              ,[THUMBNAIL]
                                          FROM [dbo].[POST_CATE]
                                    "));
                var Types = connection.Query<POST_TYPE>(string.Format(@"
                                           SELECT [ID]
                                               ,[NAME_TYPE]
                                               ,[DESCRIP]
                                               ,[THUMBNAIL]
                                               ,[ID_CATE]
                                           FROM [dbo].[POST_TYPE]
                                    "));
                // ViewBag.CateList = new SelectList(Cates, "ID", "CATE_NAME");
                ViewBag.CateList = Cates;
                //ViewBag.TypeList = new SelectList(Types, "ID", "NAME_TYPE");
                ViewBag.TypeList = Types;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostInsert(POST model, HttpPostedFileBase AVARTAR)
        {
            using (connection)
            {
                var Cates = connection.Query<POST_CATE>(string.Format(@"
                                          SELECT [ID]
                                              ,[CATE_NAME]
                                              ,[DESCRIP]
                                              ,[THUMBNAIL]
                                          FROM [dbo].[POST_CATE]
                                    "));
                var Types = connection.Query<POST_TYPE>(string.Format(@"
                                           SELECT [ID]
                                               ,[NAME_TYPE]
                                               ,[DESCRIP]
                                               ,[THUMBNAIL]
                                               ,[ID_CATE]
                                           FROM [dbo].[POST_TYPE]
                                    "));
                ViewBag.CateList = Cates;
                ViewBag.TypeList = Types;
                if (ModelState.IsValid && AVARTAR != null)
                {
                    string ext = Path.GetExtension(AVARTAR.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        string pic = Path.GetFileName(AVARTAR.FileName);
                        string path = Path.Combine(Server.MapPath("~/img"), pic);
                        AVARTAR.SaveAs(path);
                        var query = string.Format(@"
                            INSERT INTO [dbo].[POST]
                                  ([TITLE]
                                  ,[CONTENT]
                                  ,[AVARTAR]
                                  ,[OPTIONAL]
                                  ,[ACTIVE]
                                  ,[ID_TYPE]
                                  ,[CATE_ID]
                                  ,[PRICE]
                                  ,[SEOURL])
                            VALUES(
                                  N'{0}'
                                  ,N'{1}'
                                  ,N'{2}'
                                  ,N'{3}'
                                  ,{4}
                                  ,{5}
                                  ,{6}
                                  ,{7}
                                  ,N'{8}'
                                  )
                ", model.TITLE, model.CONTENT, pic,
                        model.OPTIONAL, model.ACTIVE ? 1 : 0, model.ID_TYPE, model.CATE_ID,
                        model.PRICE, TonberryKing.Seourl(model.TITLE));
                        connection.Execute(query);
                        return RedirectToAction("Post");
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng chọn ảnh để làm avatar";
                        return View("PostInsertView", model);
                    }
                }
                return View("PostInsertView", model);
            }
        }
        #endregion

        #region Update
        [HttpGet]
        public ActionResult PostUpdateView(int id)
        {
            using (connection)
            {
                var Cates = connection.Query<POST_CATE>(string.Format(@"
                                          SELECT [ID]
                                              ,[CATE_NAME]
                                              ,[DESCRIP]
                                              ,[THUMBNAIL]
                                          FROM [dbo].[POST_CATE]
                                    "));
                var Types = connection.Query<POST_TYPE>(string.Format(@"
                                           SELECT [ID]
                                               ,[NAME_TYPE]
                                               ,[DESCRIP]
                                               ,[THUMBNAIL]
                                               ,[ID_CATE]
                                           FROM [dbo].[POST_TYPE]
                                    "));
                ViewBag.CateList = Cates;
                ViewBag.TypeList = Types;

                var models = connection.Query<POST>(string.Format(@"
                                           SELECT Post.[ID], Post.[TITLE], Post.[CONTENT], Post.[AVARTAR], Post.[ID_TYPE], Post.[CATE_ID], Post.[PRICE], Post.[SEOURL], Post.[ACTIVE],
                               	      Cate.[ID], Cate.[CATE_NAME], Cate.[DESCRIP], Cate.[THUMBNAIL],
                               	      Typ.[ID], Typ.[NAME_TYPE], Typ.[THUMBNAIL],Typ.[ID_CATE]
                               FROM [dbo].[POST] Post
                               	INNER JOIN [dbo].[POST_CATE] Cate ON Post.CATE_ID = Cate.ID
                               	INNER JOIN [dbo].[POST_TYPE] Typ ON  Typ.ID = Post.ID_TYPE
                                          WHERE Post.[ID] = {0}", id));
                return View(models.FirstOrDefault());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostUpdate(POST model, HttpPostedFileBase AVARTAR)
        {
            using (connection)
            {
                var Cates = connection.Query<POST_CATE>(string.Format(@"
                                          SELECT [ID]
                                              ,[CATE_NAME]
                                              ,[DESCRIP]
                                              ,[THUMBNAIL]
                                          FROM [dbo].[POST_CATE]
                                    "));
                var Types = connection.Query<POST_TYPE>(string.Format(@"
                                           SELECT [ID]
                                               ,[NAME_TYPE]
                                               ,[DESCRIP]
                                               ,[THUMBNAIL]
                                               ,[ID_CATE]
                                           FROM [dbo].[POST_TYPE]
                                    "));
                ViewBag.CateList = Cates;
                ViewBag.TypeList = Types;
                if (ModelState.IsValid && AVARTAR != null)
                {
                    string ext = Path.GetExtension(AVARTAR.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        string pic = Path.GetFileName(AVARTAR.FileName);
                        string path = Path.Combine(Server.MapPath("~/img"), pic);
                        AVARTAR.SaveAs(path);
                        model.AVARTAR = @"~/img/" + pic;
                        var query = string.Format(@"
                            UPDATE [dbo].[POST]
                            SET [TITLE] = N'{1}'
                               ,[CONTENT] = N'{2}'
                               ,[AVARTAR] = N'{3}'
                               ,[OPTIONAL] = N'{4}'
                               ,[ACTIVE] = {5}
                               ,[ID_TYPE] = {6}
                               ,[CATE_ID] = {7}
                               ,[PRICE] = {8}
                               ,[SEOURL] = N'{9}'
                          WHERE [ID] = {0}
                ", model.ID, model.TITLE, model.CONTENT, pic,
                        model.OPTIONAL, model.ACTIVE ? 1 : 0, model.ID_TYPE, model.CATE_ID,
                        model.PRICE, TonberryKing.Seourl(model.TITLE));
                        connection.Execute(query);
                        return RedirectToAction("Post");
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng chọn ảnh để làm avatar";
                        return View("PostUpdateView", model);
                    }
                }
                return View("PostUpdateView", model);
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostDelete(int postID)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                    DELETE FROM [dbo].[POST]
                          WHERE [ID] = {0}",
                          postID));
                    return RedirectToAction("Post");
                }
                return RedirectToAction("Post");
            }
        }
        #endregion
        #endregion

        #region URL
        [HttpGet]
        public ActionResult UrlView()
        {
            using (connection)
            {
                var modules = connection.Query<URL>(string.Format(@"
                              SELECT [ID]
                                    ,[NAME]
                                    ,[DESCRIP]
                              FROM [dbo].[URL]
                              WHERE [ID] = 1
	                          ")).FirstOrDefault();
                return View(modules);
            }
        }

        [HttpPost]
        public ActionResult UrlUpdate(URL model)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    var modules = connection.Query<URL>(string.Format(@"
                             UPDATE [dbo].[URL]
                             SET [DESCRIP] = N'{0}'
                             WHERE [ID] = 1
	                          ", model.DESCRIP)).FirstOrDefault();
                    return RedirectToAction("Index");
                }
                return View("UrlView", model);
            }
        }
        #endregion
    }
}