using Avidclan_Website.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

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
        [Route("home/")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("about-us/")]
        public ActionResult AboutUs()
        {
            return View();
        }

        [Route("services/")]
        public ActionResult Services()
        {
            return View();
        }

        [Route("technologies/")]
        public ActionResult Technologies()
        {
            return View();
        }

        [Route("portfolio/")]
        public ActionResult Portfolio()
        {
            return View();
        }

        [Route("career/")]
        public ActionResult Career()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var CareerList = con.Query<Career>("sp_Careers", parameters, commandType: CommandType.StoredProcedure);
            return View(CareerList);
        }

        [Route("contact-us/")]
        public ActionResult ContactUs()
        {
            return View();
        }

        [Route("web-development-services/")]
        public ActionResult WebDevelopment()
        {
            return View();
        }

        [Route("mobile-application-development/")]
        public ActionResult MobileApplication()
        {
            return View();
        }

        [Route("design/")]
        public ActionResult Design()
        {
            return View();
        }

        [Route("internet-marketing-services/")]
        public ActionResult InternetMarketing()
        {
            return View();
        }

        [Route("quality-assurance-services/")]
        public ActionResult QualityAssurance()
        {
            return View();
        }

        [Route("iot-service-providers/")]
        public ActionResult InternetofThings()
        {
            return View();
        }

        [Route("angular-development-services/")]
        public ActionResult AngularDevelopment()
        {
            return View();
        }

        [Route("reactjs-development-services/")]
        public ActionResult ReactJsDevelopment()
        {
            return View();
        }

        [Route("vuejs-development-services/")]
        public ActionResult VueJsDevelopment()
        {
            return View();
        }

        [Route("typescript-web-development/")]
        public ActionResult TypeScriptDevelopment()
        {
            return View();
        }

        [Route("html5-development/")]
        public ActionResult Html5Development()
        {
            return View();
        }

        [Route("dot-net-development-company/")]
        public ActionResult DotNetDevelopment()
        {
            return View();
        }

        [Route("java-development-company/")]
        public ActionResult JavaDevelopment()
        {
            return View();
        }

        [Route("php-development-company/")]
        public ActionResult PhpDevelopment()
        {
            return View();
        }

        [Route("nodejs-development-services/")]
        public ActionResult NodeJsDevelopment()
        {
            return View();
        }

        [Route("ios-app-development-company/")]
        public ActionResult IosAppDevelopment()
        {
            return View();
        }

        [Route("android-app-development-company/")]
        public ActionResult AndroidAppDevelopment()
        {
            return View();
        }

        [Route("react-native-app-development/")]
        public ActionResult ReactNativeDevelopment()
        {
            return View();
        }

        [Route("flutter-app-development-company/")]
        public ActionResult FlutterAppDevelopment()
        {
            return View();
        }

        [Route("ionic-app-development-company/")]
        public ActionResult IonicAppDevelopment()
        {
            return View();
        }

        [Route("xamarin-app-development-company/")]
        public ActionResult XamarinAppDevelopment()
        {
            return View();
        }

        [Route("aws-development-services/")]
        public ActionResult AwsDevelopment()
        {
            return View();
        }

        [Route("google-cloud-services/")]
        public ActionResult GoogleCloudServices()
        {
            return View();
        }

        [Route("microsoft-azure-development/")]
        public ActionResult MicrosoftAzureDevelopment()
        {
            return View();
        }

        [Route("jenkins-management-services/")]
        public ActionResult JenkinsManagementServices()
        {
            return View();
        }

        [Route("selenium-automation-testing-services/")]
        public ActionResult SeleniumAutomationTestingService()
        {
            return View();
        }

        [Route("microsoft-sql-server-manage/")]
        public ActionResult MicrosoftSqlServerManage()
        {
            return View();
        }

        [Route("mysql-development-company/")]
        public ActionResult MySqlDevelopment()
        {
            return View();
        }

        [Route("postgresql-development-company/")]
        public ActionResult PostgreSqlDevelopment()
        {
            return View();
        }

        [Route("firebase-development-company/")]
        public ActionResult FirebaseDevelopment()
        {
            return View();
        }

        [Route("mongodb-development-company/")]
        public ActionResult MongodbDevelopment()
        {
            return View();
        }

        [Route("redis-development/")]
        public ActionResult RedisDevelopment()
        {
            return View();
        }

        [Route("microsoft-azure-cosmos-db/")]
        public ActionResult MicrosoftAzureCosmos()
        {
            return View();
        }

        [Route("microsoft-dynamics-ax-development/")]
        public ActionResult MicrosoftDynamicsAXDevelopment()
        {
            return View();
        }

        [Route("microsoft-dynamics-crm-development/")]
        public ActionResult MicrosoftDynamicsCRMDevelopment()
        {
            return View();
        }

        [Route("sharepoint-development-company/")]
        public ActionResult SharePointDevelopment()
        {
            return View();
        }

        [Route("life-at-avidclan/")]
        public ActionResult LifeAtAvidclan()
        {
            return View();
        }

        [Route("thankyou")]
        public ActionResult ThankYou()
        {
            return View();
        }

        [Route("blog/")]
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
                    blog.Id = BlogList[i].Id;
                    blog.Title = BlogList[i].Title;
                    blog.Description = BlogList[i].Description;
                    blog.BlogType = BlogList[i].BlogType;
                    blog.Image = BlogList[i].Image;
                    blog.PostingDate = BlogList[i].PostingDate;
                    blog.PostedBy = BlogList[i].PostedBy;
                    blog.PageUrl = BlogList[i].PageUrl;
                    blog.MetaTitle = BlogList[i].MetaTitle;
                    blog.MetaDescription = BlogList[i].MetaDescription;
                    listBlog.Add(blog);
                }

                var pager = new Pager((BlogList != null && BlogList.Count > 0) ? Convert.ToInt32(BlogList[0].TotalRecords) : 0, page, PageSize);
                obj.ListBlog = listBlog;
                obj.pager = pager;

                 //result = CreateBlogPage(BlogList);
            }
            return JsonConvert.SerializeObject(new { Isvalid = true, data = obj });
        }

        public string CreateBlogPage(List<Blog> blogList)
        {
            string result = string.Empty;
            try
            {
                foreach (var blog in blogList)
                {
                    //string path = @"P:\Avidclan\Avidclan\Avidclan_Website\Views\BlogPages\" + blog.PageUrl + ".cshtml";
                    //string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/BlogPages/");
                    //string targetPath = Path.Combine(targetFolder, blog.PageUrl + ".cshtml");
                    blog.Description += "@{Layout = \"~/Views/Shared/_blogdetail.cshtml\";}";
                    blog.Description += "@section AdditionalMeta{<title>" + blog.MetaTitle + "</title><meta name=\"description\" content=\" " + blog.MetaDescription + " \">}";
                     var mappedPath = HostingEnvironment.MapPath("~/Views/BlogPages/" + blog.PageUrl + ".cshtml");
                    //var mappedPath = @"https://www.avidclan.com/";

                    using (StreamWriter sw = new StreamWriter(mappedPath))
                    {
                        sw.Write(blog.Description);
                    }
                    result = "true";
                }
            }
            catch(Exception ex)
            {
                result = ex.Message+ex.StackTrace;
                
            }
            return result;

        }


        [Route("blog-details/")]
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

        [Route("pagenotfound/")]
        public ActionResult PageNotFound()
        {
            return View();
        }

        public JsonResult GetBlogData(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
            var reader = con.QueryMultiple("sp_Blog", parameters, commandType: CommandType.StoredProcedure);
            var bloglist = reader.Read<Blog>().ToList();
            var blogfaqslist = reader.Read<BlogFaqs>().ToList();
            var dynamiclist = new
            {
                bloglist = bloglist,
                blogfaqslist = blogfaqslist
            };
            return Json(dynamiclist, JsonRequestBehavior.AllowGet);
        }

    }
}