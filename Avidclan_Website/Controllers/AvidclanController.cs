using Avidclan_Website.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Avidclan_Website.Controllers
{
    public class AvidclanController : Controller
    {

        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public AvidclanController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: Avidclan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult Technologies()
        {
            return View();
        }

        public ActionResult Portfolio()
        {
            return View();
        }

        public ActionResult Career()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var CareerList = con.Query<Career>("sp_Careers", parameters, commandType: CommandType.StoredProcedure);
            return View(CareerList);
        }

        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult WebDevelopment()
        {
            return View();
        }

        public ActionResult MobileApplication()
        {
            return View();
        }

        public ActionResult Design()
        {
            return View();
        }

        public ActionResult InternetMarketing()
        {
            return View();
        }

        public ActionResult QualityAssurance()
        {
            return View();
        }

        public ActionResult InternetofThings()
        {
            return View();
        }

        public ActionResult AngularDevelopment()
        {
            return View();
        }

        public ActionResult ReactJsDevelopment()
        {
            return View();
        }

        public ActionResult VueJsDevelopment()
        {
            return View();
        }

        public ActionResult TypeScriptDevelopment()
        {
            return View();
        }

        public ActionResult Html5Development()
        {
            return View();
        }

        public ActionResult DotNetDevelopment()
        {
            return View();
        }

        public ActionResult JavaDevelopment()
        {
            return View();
        }

        public ActionResult PhpDevelopment()
        {
            return View();
        }

        public ActionResult NodeJsDevelopment()
        {
            return View();
        }

        public ActionResult IosAppDevelopment()
        {
            return View();
        }

        public ActionResult AndroidAppDevelopment()
        {
            return View();
        }

        public ActionResult ReactNativeDevelopment()
        {
            return View();
        }

        public ActionResult FlutterAppDevelopment()
        {
            return View();
        }

        public ActionResult IonicAppDevelopment()
        {
            return View();
        }

        public ActionResult XamarinAppDevelopment()
        {
            return View();
        }

        public ActionResult AwsDevelopment()
        {
            return View();
        }

        public ActionResult GoogleCloudServices()
        {
            return View();
        }

        public ActionResult MicrosoftAzureDevelopment()
        {
            return View();
        }

        public ActionResult JenkinsManagementServices()
        {
            return View();
        }

        public ActionResult SeleniumAutomationTestingService()
        {
            return View();
        }

        public ActionResult MicrosoftSqlServerManage()
        {
            return View();
        }

        public ActionResult MySqlDevelopment()
        {
            return View();
        }

        public ActionResult PostgreSqlDevelopment()
        {
            return View();
        }

        public ActionResult FirebaseDevelopment()
        {
            return View();
        }

        public ActionResult MongodbDevelopment()
        {
            return View();
        }

        public ActionResult RedisDevelopment()
        {
            return View();
        }

        public ActionResult MicrosoftAzureCosmos()
        {
            return View();
        }

        public ActionResult MicrosoftDynamicsAXDevelopment()
        {
            return View();
        }

        public ActionResult MicrosoftDynamicsCRMDevelopment()
        {
            return View();
        }

        public ActionResult SharePointDevelopment()
        {
            return View();
        }

        public ActionResult LifeAtAvidclan()
        {
            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult Blog(int page = 1)
        {
            int PageSize = 6;
            BlogViewModel obj = new BlogViewModel();
            List<Blog> listBlog = new List<Blog>();
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Offset", (page - 1) * PageSize);
            parameters.Add("@PagingSize", PageSize);
            var BlogList = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            if (BlogList != null && BlogList.Count > 0)
            {
                for (int i = 0; i < BlogList.Count; i++)
                {
                    Blog blog = new Blog();
                    blog.Title = BlogList[i].Title;
                    blog.Description = BlogList[i].Description;
                    blog.BlogType = BlogList[i].BlogType;
                    blog.Image = BlogList[i].Image;
                    blog.PostingDate = BlogList[i].PostingDate;
                    blog.PostedBy = BlogList[i].PostedBy;
                    listBlog.Add(blog);
                }

                var pager = new Pager((BlogList != null && BlogList.Count > 0) ? Convert.ToInt32(BlogList[0].TotalRecords) : 0, page, PageSize);
                obj.ListBlog = listBlog;
                obj.pager = pager;
            }
            return View(obj);
        }


        public string GetSectionData(int page = 1)
        {
            int PageSize = 6;
            BlogViewModel obj = new BlogViewModel();
            List<Blog> listBlog = new List<Blog>();
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Offset", (page - 1) * PageSize);
            parameters.Add("@PagingSize", PageSize);
            var BlogList = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            if (BlogList != null && BlogList.Count > 0)
            {
                for (int i = 0; i < BlogList.Count; i++)
                {
                    Blog blog = new Blog();
                    blog.Title = BlogList[i].Title;
                    blog.Description = BlogList[i].Description;
                    blog.BlogType = BlogList[i].BlogType;
                    blog.Image = BlogList[i].Image;
                    blog.PostingDate = BlogList[i].PostingDate;
                    blog.PostedBy = BlogList[i].PostedBy;
                    listBlog.Add(blog);
                }

                var pager = new Pager((BlogList != null && BlogList.Count > 0) ? Convert.ToInt32(BlogList[0].TotalRecords) : 0, page, PageSize);
                obj.ListBlog = listBlog;
                obj.pager = pager;
            }
            return JsonConvert.SerializeObject(new { Isvalid = true, data = obj });
        }

        public ActionResult BlogDetails()
        {
            return View();
        }

        public JsonResult GetCareerOption()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            var CareerList = con.Query<Career>("sp_Careers", parameters, commandType: CommandType.StoredProcedure);
            return Json(CareerList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRolesResponsiblity(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
            var CareerList = con.Query<Career>("sp_Careers", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return Json(CareerList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetBlogStatusByName(string name)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MenuName", name, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var menuDetail = con.Query<HeaderMenu>("sp_TblMenu", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            if (menuDetail == null)
            {
                menuDetail = new HeaderMenu();
            }
            return Json(menuDetail, JsonRequestBehavior.AllowGet);
        }

    }
}