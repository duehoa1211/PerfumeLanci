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
    //[CompressContent]
    public class AdminController : Controller
    {
        IDbConnection connection;
        public AdminController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
        #region Cart Management
        [HttpGet]
        public ActionResult Index()
        {
            using (connection)
            {
                var modules = connection.Query<CART>(string.Format(@"
                            SELECT [BILL_ID]
                                ,[INFOS]
                                ,[CUSTOMER]
                                ,[ADDRESS]
                                ,[PHONENUMBER]
                                ,[EMAIL]
                            FROM [dbo].[CART]
                    "));
                if (!(modules.Count() > 0))
                {
                    modules = null;
                }
                return View(modules);
            }
        }
        #region Update Cart
        [HttpGet]
        public ActionResult Detail(int id)
        {
            var modules = connection.Query<CART, CART_DETAIL, POST, CART>(string.Format(@"
                        SELECT CART.[BILL_ID],[INFOS],[CUSTOMER],[ADDRESS],[PHONENUMBER],[EMAIL],
    	                       DETAIL.[BILL_ID]  ,[PRODUCT_ID]  ,[QUANTITY],
                               PRODUCT.[ID],  PRODUCT.[TITLE], PRODUCT.[AVARTAR]
                        FROM [dbo].[CART] CART
	                         INNER JOIN [dbo].[CART_DETAIL] DETAIL ON CART.BILL_ID = DETAIL.BILL_ID
                             INNER JOIN [dbo].[POST] PRODUCT ON PRODUCT.[ID] = DETAIL.[PRODUCT_ID]
                        WHERE CART.[BILL_ID] = {0}", id),
                        (Cart, Detail, Post) =>
                        {
                            Cart.CartDetail = Cart.CartDetail ?? new List<CART_DETAIL>();
                            Detail.Product = Post ?? new POST();
                            if (Detail != null)
                            {
                                Cart.CartDetail.Add(Detail);
                            }

                            return Cart;
                        }, splitOn: "BILL_ID,ID").GroupBy(z => z.BILL_ID).Select(lstCart =>
                        {
                            var cart = lstCart.FirstOrDefault();
                            cart.CartDetail = lstCart.Select(z => z.CartDetail.FirstOrDefault()).ToList();
                            return cart;
                        }).FirstOrDefault();
            return View(modules);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCart(CART model)
        {
            if (ModelState.IsValid)
            {
                connection.Execute(string.Format(@"
                    UPDATE [dbo].[CART]
                    SET [INFOS] = N'{1}'
                        ,[CUSTOMER] = N'{2}'
                        ,[ADDRESS] = N'{3}'
                        ,[PHONENUMBER] = N'{4}'
                        ,[EMAIL] = N'{5}'
                    WHERE [BILL_ID] = {0}
            ", model.BILL_ID, model.INFOS, model.CUSTOMER, model.ADDRESS, model.PHONENUMBER, model.EMAIL));
                return RedirectToAction("Index", "Admin");
            }
            var modules = connection.Query<CART, CART_DETAIL, POST, CART>(string.Format(@"
                        SELECT CART.[BILL_ID],[INFOS],[CUSTOMER],[ADDRESS],[PHONENUMBER],[EMAIL],
    	                       DETAIL.[BILL_ID]  ,[PRODUCT_ID]  ,[QUANTITY],
                               PRODUCT.[ID],  PRODUCT.[TITLE], PRODUCT.[AVARTAR]
                        FROM [dbo].[CART] CART
	                         INNER JOIN [dbo].[CART_DETAIL] DETAIL ON CART.BILL_ID = DETAIL.BILL_ID
                             INNER JOIN [dbo].[POST] PRODUCT ON PRODUCT.[ID] = DETAIL.[PRODUCT_ID]
                        WHERE CART.[BILL_ID] = {0}", model.BILL_ID),
                       (Cart, Detail, Post) =>
                       {
                           Cart.CartDetail = Cart.CartDetail ?? new List<CART_DETAIL>();
                           Detail.Product = Post ?? new POST();
                           if (Detail != null)
                           {
                               Cart.CartDetail.Add(Detail);
                           }
                           return Cart;
                       }, splitOn: "BILL_ID,ID").GroupBy(z => z.BILL_ID).Select(lstCart =>
                       {
                           var cart = lstCart.FirstOrDefault();
                           cart.CartDetail = lstCart.Select(z => z.CartDetail.FirstOrDefault()).ToList();
                           return cart;
                       }).FirstOrDefault();
            model.CartDetail = modules.CartDetail;
            return View("Detail", model);
        }
        #endregion
        #region Delete Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCart(int id)
        {
            if (ModelState.IsValid)
            {
                using (connection)
                {
                    connection.Execute(string.Format(@"
                        DELETE FROM [dbo].[CART_DETAIL]
                        WHERE [BILL_ID] = {0}
                ", id));
                    connection.Execute(string.Format(@"
                        DELETE FROM [dbo].[CART]
                        WHERE [BILL_ID] = {0}
                ", id));
                }
            }
            return RedirectToAction("Index", "Admin");
        }
        #endregion
        #region UpdateCartDetail
        [HttpGet]
        public ActionResult UpdateProdDetail(int id, int proid)
        {
            using (connection)
            {
                var modules = connection.QueryFirstOrDefault<CART_DETAIL>(string.Format(@"
                            SELECT [BILL_ID]
                                ,[PRODUCT_ID]
                                ,[QUANTITY]
                            FROM [dbo].[CART_DETAIL]
                            WHERE [BILL_ID] = {0} AND [PRODUCT_ID] = {1}",
                            id, proid));

                var list = connection.Query<POST>(string.Format(@"
                        SELECT [ID], [TITLE]
                        FROM [dbo].[POST]
                "));
                modules.ProductList = new SelectList(list, "ID", "TITLE");
                return View(modules);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProdDet(CART_DETAIL models, int prodID)
        {
            if (ModelState.IsValid)
            {
                using (connection)
                {
                    connection.Execute(string.Format(@"
                          UPDATE [dbo].[CART_DETAIL]
                          SET [PRODUCT_ID] = {2}
                             ,[QUANTITY] = {3}
                          WHERE [BILL_ID] = {0} AND [PRODUCT_ID] = {1}
                    ", models.BILL_ID, prodID, models.PRODUCT_ID, models.QUANTITY));
                }
                return RedirectToAction("Detail", new { id = models.BILL_ID });
            }
            return RedirectToAction("UpdateProdDetail", new { id = models.BILL_ID, proid = prodID });
        }
        #endregion
        #region DeleteCartDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int ProId, int CartId)
        {
            if (ModelState.IsValid)
            {
                using (connection)
                {
                    connection.Execute(string.Format(@"
                        DELETE FROM [dbo].[CART_DETAIL]
                        WHERE [BILL_ID] = {0} AND [PRODUCT_ID] = {1}
                    ", CartId, ProId));
                }
            }
            return RedirectToAction("Detail", "Admin", new { id = CartId });
        }
        #endregion
        #endregion

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
        [ValidateAntiForgeryToken]
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
        public ActionResult PostInsert(POST model, HttpPostedFileBase image)
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
                if (ModelState.IsValid && image != null)
                {
                    string ext = Path.GetExtension(image.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        string pic = Path.GetFileName(image.FileName);
                        string path = Path.Combine(Server.MapPath("~/img/images"), pic);
                        image.SaveAs(path);
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

                var models = connection.QueryFirstOrDefault<POST>(string.Format(@"
                                           SELECT Post.[ID], Post.[TITLE], Post.[CONTENT], Post.[AVARTAR], Post.[ID_TYPE], Post.[OPTIONAL] ,Post.[CATE_ID], Post.[PRICE], Post.[SEOURL], Post.[ACTIVE],
                               	      Cate.[ID], Cate.[CATE_NAME], Cate.[DESCRIP], Cate.[THUMBNAIL],
                               	      Typ.[ID], Typ.[NAME_TYPE], Typ.[THUMBNAIL],Typ.[ID_CATE]
                               FROM [dbo].[POST] Post
                               	INNER JOIN [dbo].[POST_CATE] Cate ON Post.CATE_ID = Cate.ID
                               	INNER JOIN [dbo].[POST_TYPE] Typ ON  Typ.ID = Post.ID_TYPE
                                          WHERE Post.[ID] = {0}", id));
                return View(models);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostUpdate(POST model, HttpPostedFileBase image)
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
                if (ModelState.IsValid)
                {
                    if (image != null)
                    {
                        string ext = Path.GetExtension(image.FileName).ToLower();
                        if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                        {
                            string pic = Path.GetFileName(image.FileName);
                            string path = Path.Combine(Server.MapPath("~/img/images"), pic);
                            image.SaveAs(path);
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
                    else
                    {
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
                ", model.ID, model.TITLE, model.CONTENT, model.AVARTAR,
                           model.OPTIONAL, model.ACTIVE ? 1 : 0, model.ID_TYPE, model.CATE_ID,
                           model.PRICE, TonberryKing.Seourl(model.TITLE));
                        connection.Execute(query);
                        return RedirectToAction("Post");
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
	                          "));
                return View(modules);
            }
        }

        [HttpGet]
        public ActionResult PartialUrl()
        {
            return PartialView("ModalUrl");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UrlUpdate(URL model)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    var module = connection.Execute(string.Format(@"
                             UPDATE [dbo].[URL]
                             SET [DESCRIP] = N'{0}'
	                         WHERE [ID] = {1} ", model.DESCRIP, model.ID));
                    return RedirectToAction("UrlView");
                }
                return RedirectToAction("UrlView");
            }
        }
        #endregion

        #region OptionalSetting
        #region Intro
        [HttpGet]
        public ActionResult Intro()
        {
            using (connection)
            {
                var modules = connection.QueryFirstOrDefault<OPTIONAL>(string.Format(@"
                          SELECT [ID]
                              ,[NAME]
                              ,[CONTENTS]
                          FROM [dbo].[Optional] 
                          WHERE [ID] = 1
                "));
                return View(modules);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IntroUpdate(OPTIONAL model)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    connection.Execute(string.Format(@"
                            UPDATE [dbo].[Optional]
                            SET [CONTENTS] = N'{0}'
                            WHERE [ID] = 1
                    ", model.CONTENTS));
                    return RedirectToAction("Intro");
                }
                return View("Intro", model);
            }
        }
        #endregion

        #region Optional
        [HttpGet]
        public ActionResult Optional()
        {
            using (connection)
            {
                var modules = connection.Query<OPTIONAL>(string.Format(@"
                          SELECT [ID]
                              ,[NAME]
                              ,[CONTENTS]
                          FROM [dbo].[Optional] 
                          WHERE [ID] != 1
                "));
                return View(modules);
            }
        }

        [HttpGet]
        public ActionResult OptPartial(int id)
        {
            using (connection)
            {
                var modules = connection.QueryFirstOrDefault<OPTIONAL>(string.Format(@"
                          SELECT [ID]
                              ,[NAME]
                              ,[CONTENTS]
                          FROM [dbo].[Optional] 
                          WHERE [ID] = {0}
                ", id));
                return View(modules);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOpt(OPTIONAL model, HttpPostedFileBase image)
        {
            using (connection)
            {
                if (ModelState.IsValid && image != null)
                {
                    string ext = Path.GetExtension(image.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        string pic = Path.GetFileName(image.FileName);
                        string path = Path.Combine(Server.MapPath("~/img/images"), pic);
                        image.SaveAs(path);
                        connection.Execute(string.Format(@"
                            UPDATE [dbo].[Optional]
                            SET [CONTENTS] = N'{1}'
                            WHERE [ID] = {0}
                        ", model.ID, pic));
                        return RedirectToAction("Optional");
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng chọn ảnh";
                        return View("PostInsertView", model);
                    }
                }
                return View("OptPartial", model);
            }
        }
        #endregion

        #endregion

        #region Slider
        [HttpGet]
        public ActionResult Sliders()
        {
            using (connection)
            {
                var modules = connection.Query<SLIDER>(string.Format(@"
                                SELECT [ID]
                                    ,[NAME]
                                    ,[URI]
                                    ,[IMAGE]
                                FROM [dbo].[SLIDER]"
                ));
                return View(modules);
            }
        }
        #region Insert
        [HttpGet]
        public ActionResult SliderInsertView()
        {
            using (connection)
            {
                var list = connection.Query<POST>(string.Format(@"
                            SELECT [ID]
                                ,[TITLE]
                            FROM [dbo].[POST]
                            "));
                var model = new SLIDER();
                model.ListSanPham = new SelectList(list, "ID", "TITLE");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SliderInsert(SLIDER model, HttpPostedFileBase images)
        {
            using (connection)
            {
                if (ModelState.IsValid && images != null)
                {
                    string ext = Path.GetExtension(images.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        string pic = Path.GetFileName(images.FileName);
                        string path = Path.Combine(Server.MapPath("~/img/images"), pic);
                        images.SaveAs(path);
                        connection.Execute(string.Format(@"
                           INSERT INTO [dbo].[SLIDER]
                                ([NAME]
                                ,[URI]
                                ,[IMAGE])
                            VALUES(
                                N'{0}'
                                ,N'{1}'
                                ,N'{2}')
                        ", model.NAME, model.URI, pic));
                        return RedirectToAction("Sliders");
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng chọn ảnh";
                        return View("SliderInsertView", model);
                    }
                }
                var list = connection.Query<POST>(string.Format(@"
                            SELECT [ID]
                                ,[TITLE]
                            FROM [dbo].[POST]
                            "));
                model.ListSanPham = new SelectList(list, "ID", "TITLE");
                return View("SliderInsertView", model);
            }
        }
        #endregion
        #region Update
        [HttpGet]
        public ActionResult SliderUpdateView(int id)
        {
            using (connection)
            {
                var models = connection.QueryFirstOrDefault<SLIDER>(string.Format(@"
                                SELECT [ID]
                                    ,[NAME]
                                    ,[URI]
                                    ,[IMAGE]
                                FROM [dbo].[SLIDER]
                                WHERE [ID] = {0}", id
                ));
                var list = connection.Query<POST>(string.Format(@"
                            SELECT [ID]
                                ,[TITLE]
                            FROM [dbo].[POST]
                            "));
                models.ListSanPham = new SelectList(list, "ID", "TITLE");
                return View(models);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SliderUpdate(SLIDER model, HttpPostedFileBase images)
        {
            using (connection)
            {
                if (ModelState.IsValid)
                {
                    if (images != null)
                    {
                        string ext = Path.GetExtension(images.FileName).ToLower();
                        if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                        {
                            string pic = Path.GetFileName(images.FileName);
                            string path = Path.Combine(Server.MapPath("~/img/images"), pic);
                            images.SaveAs(path);
                            connection.Execute(string.Format(@"
                           UPDATE [dbo].[SLIDER]
                           SET [NAME] = N'{1}'
                                ,[URI] = N'{2}'
                                ,[IMAGE] = N'{3}'
                           WHERE [ID] = {0}
                        ", model.ID, model.NAME, model.URI, pic));
                            return RedirectToAction("Sliders");
                        }
                        else
                        {
                            ViewBag.Error = "Vui lòng chọn ảnh";
                            return View("SliderUpdateView", model);
                        }
                    }
                    else
                    {
                        connection.Execute(string.Format(@"
                           UPDATE [dbo].[SLIDER]
                           SET [NAME] = N'{1}'
                                ,[URI] = N'{2}'
                                ,[IMAGE] = N'{3}'
                           WHERE [ID] = {0}
                        ", model.ID, model.NAME, model.URI, model.IMAGE));
                        return RedirectToAction("Sliders");
                    }
                }
                var list = connection.Query<POST>(string.Format(@"
                            SELECT [ID]
                                ,[TITLE]
                            FROM [dbo].[POST]
                            "));
                model.ListSanPham = new SelectList(list, "ID", "TITLE");
                return View("SliderUpdateView", model);
            }
        }
        #endregion
        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSlide(int id)
        {
            using (connection)
            {
                connection.Execute(string.Format(@"
                       DELETE FROM [dbo].[SLIDER]
                       WHERE [ID] = {0}
                ", id));
                return RedirectToAction("Sliders", "Admin");
            }
        }
        #endregion
        #endregion
    }
}
