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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
            if (model != null)
            {
                if (model.CartDetail.Count == 0)
                {
                    model = null;
                }
            }
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
<<<<<<< HEAD
        public ActionResult AddItem(int id)
=======
<<<<<<< HEAD
        public ActionResult AddItem(int id)
=======
        public ActionResult AddItem(int ID, int quantity)
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
        {
            if (ModelState.IsValid)
            {
                using (connection)
                {
                    var product = connection.QueryFirstOrDefault<POST>(string.Format(@"
                            SELECT [ID], [TITLE], [PRICE]
                            FROM [dbo].[POST]
                            WHERE [ID] = {0}
<<<<<<< HEAD
                ", id));
=======
<<<<<<< HEAD
                ", id));
=======
                ", ID));
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
                    if (product != null)
                    {
                        var detail = new CART_DETAIL();
                        detail.Product = product;
                        detail.PRODUCT_ID = product.ID;
<<<<<<< HEAD
                        var cart = Session["Cart"] as CART ?? new CART();
=======
<<<<<<< HEAD
                        var cart = Session["Cart"] as CART ?? new CART();
=======
                        detail.QUANTITY = quantity;
                        var cart = Session["Cart"] as CART ?? new CART();
                        if (cart.CartDetail != null)
                        {
                            this.UpdateQuantity(ID, quantity);
                        }
                        cart.CartDetail = new List<CART_DETAIL>();

>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
                        cart.CartDetail.Add(detail);
                        Session["Cart"] = cart;

                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }

<<<<<<< HEAD
        [HttpPost]
        [ValidateAntiForgeryToken]
=======
<<<<<<< HEAD
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
=======
        [HttpGet]
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
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
<<<<<<< HEAD
=======
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
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
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
<<<<<<< HEAD
                    connection.Execute(string.Format(@"
=======
<<<<<<< HEAD
                    connection.Execute(string.Format(@"
=======
                    var query = string.Format(@"
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
                            INSERT INTO [dbo].[CART]
                                   ([INFOS]
                                   ,[CUSTOMER]
                                   ,[ADDRESS]
                                   ,[PHONENUMBER]
                                   ,[EMAIL])
                             VALUES
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
                                   ({0}
                                   ,{1}
                                   ,{2}
                                   ,{3}
                                   ,{4})
<<<<<<< HEAD
                    ", model.INFOS, model.CUSTOMER, model.ADDRESS,
                    model.PHONENUMBER, model.EMAIL));
=======
                    ", model.INFOS, model.CUSTOMER, model.ADDRESS,
                    model.PHONENUMBER, model.EMAIL));
=======
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
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
                    connection.Execute(string.Format(@"
                            INSERT INTO [dbo].[CART_DETAIL]
                                   ([BILL_ID]
                                   ,[PRODUCT_ID]
                                   ,[QUANTITY])
                             VALUES
                                   (@BILL_ID
                                   ,@PRODUCT_ID
<<<<<<< HEAD
                                   ,@QUANTITY
=======
<<<<<<< HEAD
                                   ,@QUANTITY
=======
                                   ,@QUANTITY)
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
                        
                    "), model.CartDetail);
                    return RedirectToAction("Index", "Home");
                }
            }
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
            model.CartDetail = cart.CartDetail;
>>>>>>> 161219b8b67f34e37dace89e894e177a72b2b3b8
>>>>>>> 7839882dfa559a4848f9157c795c3b5f3b740414
            return View("CheckOutView", model);
        }
    }
}