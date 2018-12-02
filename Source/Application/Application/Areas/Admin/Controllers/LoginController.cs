using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using TonberryLib;
using System.Configuration;
using Dapper;
using System.Web.Security;

namespace Application.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        IDbConnection dataConnection;

        public LoginController()
        {
            dataConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            using (dataConnection)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckLogin(string usernmae, string password)
        {
            using (dataConnection)
            {
                if (!string.IsNullOrEmpty(usernmae) && !string.IsNullOrEmpty(password))
                {
                    string paramName = TonberryKing.SqlSpecialCharacterNonXss(usernmae);
                    string paramPass = TonberryKing.SqlSpecialCharacterNonXss(password);

                    int countAdmin = dataConnection.Query<int>(string.Format(@"
                                        SELECT COUNT(ID)
                                          FROM [dbo].[ADMINS]
                                          WHERE [USERNAME] = '{0}'
                                          AND [PASSWORDS] = '{1}'
                                        ", paramName, paramPass)).FirstOrDefault();
                    if (countAdmin > 0)
                    {
                        FormsAuthentication.SetAuthCookie("Admin", true);
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                }
                return View("Index");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return Redirect(Url.Action("Index", "Home", new { area = "" }));
        }
    }
}