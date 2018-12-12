using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart

        // TODO: Thêm xóa sửa session đơn hàng
        public ActionResult Index()
        {
            // TODO: 
            return View();
        }

        [HttpGet]
        public ActionResult CheckOutView()
        {
            // TODO: View của thanh toán đơn hàng theo class CART
            //       
            return View();
        }

        [HttpPost]
        public ActionResult Checkout()
        {
            // TODO: Thanh toán đơn hàng aka thêm vào database 
            return View();
        }
    }
}