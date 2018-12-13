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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(int id)
        {
            if (ModelState.IsValid)
            {
                using (connection)
                {
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
                        var cart = Session["Cart"] as CART ?? new CART();
                        cart.CartDetail.Add(detail);
                        Session["Cart"] = cart;

                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as CART;
                var result = cart.CartDetail.FirstOrDefault(z => z.PRODUCT_ID == id);
                result.QUANTITY += quantity;
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
                    connection.Execute(string.Format(@"
                            INSERT INTO [dbo].[CART]
                                   ([INFOS]
                                   ,[CUSTOMER]
                                   ,[ADDRESS]
                                   ,[PHONENUMBER]
                                   ,[EMAIL])
                             VALUES
                                   ({0}
                                   ,{1}
                                   ,{2}
                                   ,{3}
                                   ,{4})
                    ", model.INFOS, model.CUSTOMER, model.ADDRESS,
                    model.PHONENUMBER, model.EMAIL));
                    connection.Execute(string.Format(@"
                            INSERT INTO [dbo].[CART_DETAIL]
                                   ([BILL_ID]
                                   ,[PRODUCT_ID]
                                   ,[QUANTITY])
                             VALUES
                                   (@BILL_ID
                                   ,@PRODUCT_ID
                                   ,@QUANTITY
                        
                    "), model.CartDetail);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View("CheckOutView", model);
        }
    }
}