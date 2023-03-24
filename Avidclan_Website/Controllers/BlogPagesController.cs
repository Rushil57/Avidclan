using Avidclan_Website.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avidclan_Website.Controllers
{
    public class BlogPagesController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public BlogPagesController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: BlogPages
        public ActionResult Index(string url)
        {
            return View();
        }

        public ActionResult blog(string id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageUrl", id, DbType.String, ParameterDirection.Input);
            var CareerList = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            ViewBag.HtmlData = CareerList.Description;
            ViewBag.MetaData = CareerList.MetaDescription;
            ViewBag.MetaTitle = CareerList.MetaTitle;
            var CanonicalUrl = "https://www.avidclan.com/blog/" + id + "/";
            ViewBag.Url = CanonicalUrl;
            return View();
        }
        //protected override void HandleUnknownAction(string actionName)
        //{
        //    this.View(actionName).ExecuteResult(this.ControllerContext);
        //}
    }
}
