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

        public ActionResult AvidclanAction()
        {
            return View("Index");
        }

        // GET: Avidclan
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("aboutus")]
        public ActionResult AboutUs()
        {
            return View();
        }

        [Route("services")]
        public ActionResult Services()
        {
            return View();
        }

        [Route("technologies")]
        public ActionResult Technologies()
        {
            return View();
        }

        [Route("portfolio")]
        public ActionResult Portfolio()
        {
            return View();
        }

        [Route("career")]
        public ActionResult Career()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var CareerList = con.Query<Career>("sp_Careers", parameters, commandType: CommandType.StoredProcedure);
            return View(CareerList);
        }

        [Route("contactus")]
        public ActionResult ContactUs()
        {
            return View();
        }

        [Route("webdevelopment")]
        public ActionResult WebDevelopment()
        {
            return View();
        }

        [Route("mobileapplication")]
        public ActionResult MobileApplication()
        {
            return View();
        }

        [Route("design")]
        public ActionResult Design()
        {
            return View();
        }

        [Route("internetmarketing")]
        public ActionResult InternetMarketing()
        {
            return View();
        }

        [Route("qualityassurance")]
        public ActionResult QualityAssurance()
        {
            return View();
        }

        [Route("internetofthings")]
        public ActionResult InternetofThings()
        {
            return View();
        }

        [Route("angulardevelopment")]
        public ActionResult AngularDevelopment()
        {
            return View();
        }

        [Route("reactjsdevelopment")]
        public ActionResult ReactJsDevelopment()
        {
            return View();
        }

        [Route("vuejsdevelopment")]
        public ActionResult VueJsDevelopment()
        {
            return View();
        }

        [Route("typescriptdevelopment")]
        public ActionResult TypeScriptDevelopment()
        {
            return View();
        }

        [Route("html5development")]
        public ActionResult Html5Development()
        {
            return View();
        }

        [Route("dotnetdevelopment")]
        public ActionResult DotNetDevelopment()
        {
            return View();
        }

        [Route("javadevelopment")]
        public ActionResult JavaDevelopment()
        {
            return View();
        }

        [Route("phpdevelopment")]
        public ActionResult PhpDevelopment()
        {
            return View();
        }

        [Route("nodejsdevelopment")]
        public ActionResult NodeJsDevelopment()
        {
            return View();
        }

        [Route("iosappdevelopment")]
        public ActionResult IosAppDevelopment()
        {
            return View();
        }

        [Route("androidappdevelopment")]
        public ActionResult AndroidAppDevelopment()
        {
            return View();
        }

        [Route("reactnativedevelopment")]
        public ActionResult ReactNativeDevelopment()
        {
            return View();
        }

        [Route("flutterappdevelopment")]
        public ActionResult FlutterAppDevelopment()
        {
            return View();
        }

        [Route("ionicappdevelopment")]
        public ActionResult IonicAppDevelopment()
        {
            return View();
        }

        [Route("xamarinappdevelopment")]
        public ActionResult XamarinAppDevelopment()
        {
            return View();
        }

        [Route("awsdevelopment")]
        public ActionResult AwsDevelopment()
        {
            return View();
        }

        [Route("googlecloudservices")]
        public ActionResult GoogleCloudServices()
        {
            return View();
        }

        [Route("microsoftazuredevelopment")]
        public ActionResult MicrosoftAzureDevelopment()
        {
            return View();
        }

        [Route("jenkinsmanagementservices")]
        public ActionResult JenkinsManagementServices()
        {
            return View();
        }

        [Route("seleniumautomationtestingservice")]
        public ActionResult SeleniumAutomationTestingService()
        {
            return View();
        }

        [Route("microsoftsqlservermanage")]
        public ActionResult MicrosoftSqlServerManage()
        {
            return View();
        }

        [Route("mysqldevelopment")]
        public ActionResult MySqlDevelopment()
        {
            return View();
        }

        [Route("postgresqldevelopment")]
        public ActionResult PostgreSqlDevelopment()
        {
            return View();
        }

        [Route("firebasedevelopment")]
        public ActionResult FirebaseDevelopment()
        {
            return View();
        }

        [Route("mongodbdevelopment")]
        public ActionResult MongodbDevelopment()
        {
            return View();
        }

        [Route("redisdevelopment")]
        public ActionResult RedisDevelopment()
        {
            return View();
        }

        [Route("microsoftazurecosmos")]
        public ActionResult MicrosoftAzureCosmos()
        {
            return View();
        }

        [Route("microsoftdynamicsaxdevelopment")]
        public ActionResult MicrosoftDynamicsAXDevelopment()
        {
            return View();
        }

        [Route("microsoftdynamicscrmdevelopment")]
        public ActionResult MicrosoftDynamicsCRMDevelopment()
        {
            return View();
        }

        [Route("sharepointdevelopment")]
        public ActionResult SharePointDevelopment()
        {
            return View();
        }

        [Route("lifeatavidclan")]
        public ActionResult LifeAtAvidclan()
        {
            return View();
        }

        [Route("thankyou")]
        public ActionResult ThankYou()
        {
            return View();
        }

        [Route("blog")]
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

        [Route("blogdetails")]
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