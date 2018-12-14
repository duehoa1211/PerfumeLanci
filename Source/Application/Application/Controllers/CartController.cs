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
    public class CartController : Controller
    {
        IDbConnection connection;
        public CartController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
        // GET: Cart

        // TODO: Thêm xóa sửa session đơn hàng
        [HttpGet]
        public ActionResult Index()
        {
            var model = new CART();
            model = Session["Cart"] as CART;
            if (model != null)
            {
                if (model.CartDetail.Count == 0)
                {
                    model = null;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(int ID, int quantity)
        {
            if (ModelState.IsValid)
            {
                using (connection)
                {
                    var product = connection.QueryFirstOrDefault<POST>(string.Format(@"
                            SELECT [ID], [TITLE], [PRICE]
                            FROM [dbo].[POST]
                            WHERE [ID] = {0}
                            ", ID));
                    if (product != null)
                    {
                        var detail = new CART_DETAIL();
                        detail.Product = product;
                        detail.PRODUCT_ID = product.ID;
                        detail.QUANTITY = quantity;
                        var cart = Session["Cart"] as CART ?? new CART();
                        if (cart.CartDetail != null)
                        {
                            UpdateQuantity(ID, quantity);
                            return RedirectToAction("Index");
                        }
                        cart.CartDetail = new List<CART_DETAIL>();
                        cart.CartDetail.Add(detail);
                        Session["Cart"] = cart;

                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public void UpdateQuantity(int id, int quantity)
        {
            var cart = Session["Cart"] as CART;
            var result = cart.CartDetail.FirstOrDefault(z => z.PRODUCT_ID == id);
            if (result != null)
            {
                result.QUANTITY += quantity;
                Session["Cart"] = cart;
                return;
            }
            var product = connection.QueryFirstOrDefault<POST>(string.Format(@"
                            SELECT [ID], [TITLE], [PRICE]
                            FROM [dbo].[POST]
                            WHERE [ID] = {0}
                ", id));
            if (product != null)
            {
                var detail = new CART_DETAIL();
                detail.Product = product;
                detail.PRODUCT_ID = product.ID;
                detail.QUANTITY = quantity;
                cart.CartDetail.Add(detail);
            }
            Session["Cart"] = cart;
        }


        [HttpGet]
        public ActionResult DeleteItem(int id)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as CART;
                var result = cart.CartDetail.FirstOrDefault(z => z.PRODUCT_ID == id);
                if (result != null)
                {
                    cart.CartDetail.Remove(result);
                }
                Session["Cart"] = cart;
            }
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult CheckOutView()
        {
            var cart = Session["Cart"] as CART;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CART model)
        {
            var cart = Session["Cart"] as CART;
            if (ModelState.IsValid)
            {
                using (connection)
                {

                    model.CartDetail = cart.CartDetail;
                    var query = string.Format(@"
                            INSERT INTO [dbo].[CART]
                                   ([INFOS]
                                   ,[CUSTOMER]
                                   ,[ADDRESS]
                                   ,[PHONENUMBER]
                                   ,[EMAIL])
                             VALUES
                                   (N'{0}'
                                   ,N'{1}'
                                   ,N'{2}'
                                   ,N'{3}'
                                   ,N'{4}')
                    ", model.INFOS, model.CUSTOMER, model.ADDRESS,
                        model.PHONENUMBER, model.EMAIL);
                    connection.Execute(query);
                    var getID = connection.Query<CART>(string.Format(@"
                                SELECT [BILL_ID]
                                FROM CART 
                                WHERE [CUSTOMER] = N'{0}' AND [PHONENUMBER] = N'{1}'
                    ", model.CUSTOMER, model.PHONENUMBER)).OrderByDescending(z => z.BILL_ID).FirstOrDefault().BILL_ID;
                    foreach (var item in model.CartDetail)
                    {
                        item.BILL_ID = getID;
                    }
                    connection.Execute(string.Format(@"
                            INSERT INTO [dbo].[CART_DETAIL]
                                   ([BILL_ID]
                                   ,[PRODUCT_ID]
                                   ,[QUANTITY])
                             VALUES
                                   (@BILL_ID
                                   ,@PRODUCT_ID
                                   ,@QUANTITY)
                        
                    "), model.CartDetail);
                    return RedirectToAction("Index", "Home");
                }
            }
            model.CartDetail = cart.CartDetail;
            return View("CheckOutView", model);
        }
    }
}