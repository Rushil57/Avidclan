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
using System.Runtime.InteropServices;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Avidclan_Website.Controllers
{
    public class AvidclanController : Controller
    {
        /*
            Note : in bellow some multiple redirection added due to seo changes do not change without 
                   respective persion permition.
         */

        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        private static string ImageServerUrl = "https://images.weserv.nl/?url=";
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
            //return View();
            return RedirectToAction("AvidclanAction", "Avidclan");
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

        [Route("flutter-app-development-company-usa/")]
        public ActionResult FlutterAppDevelopmentUSA()
        {
            return View();
        }

        [Route("web-development-services/")]
        public ActionResult WebDevelopmentRedirect()
        {
            return RedirectToAction("WebDevelopment", "Avidclan");
        }

        [Route("web-development-company/")]
        public ActionResult WebDevelopment()
        {
            return View();
        }
        [Route("ai-development-company/")]
        public ActionResult AIDevelopment()
        {
            return View();
        }
        [Route("mobile-application-development/")]
        public ActionResult MobileApplicationRedirect()
        {
            return RedirectToAction("MobileApplication", "Avidclan");
        }

        [Route("mobile-app-development-company/")]
        public ActionResult MobileApplication()
        {
            return View();
        }

        [Route("design/")]
        public ActionResult DesignRedirect()
        {
            return RedirectToAction("Design", "Avidclan");
        }

        [Route("ui-ux-design-company/")]
        public ActionResult Design()
        {
            return View();
        }

        [Route("internet-marketing-services/")]
        public ActionResult InternetMarketingRedirect()
        {
            return RedirectToAction("InternetMarketing", "Avidclan");
        }

        [Route("digital-marketing-services/")]
        public ActionResult InternetMarketing()
        {
            return View();
            //return RedirectToAction("AvidclanAction", "Avidclan");
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
        public ActionResult AngularDevelopmentRedirect()
        {
            return RedirectToAction("AngularDevelopment", "Avidclan");
        }

        [Route("angular-development-company/")]
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
        public ActionResult VueJsDevelopmentRedirect()
        {
            return RedirectToAction("VueJsDevelopment", "Avidclan");
        }

        [Route("vuejs-development-company/")]
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
        public ActionResult Html5DevelopmentRedirect()
        {
            return RedirectToAction("Html5Development", "Avidclan");
        }

        [Route("html5-development-services/")]
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
        public ActionResult NodeJsDevelopmentRedirect()
        {
            return RedirectToAction("NodeJsDevelopment", "Avidclan");
        }

        [Route("nodejs-development-company/")]
        public ActionResult NodeJsDevelopment()
        {
            return View();
        }

        [Route("ios-app-development-company/")]
        public ActionResult IosAppDevelopmentRedirect()
        {
            return RedirectToAction("IosAppDevelopment", "Avidclan");
        }

        [Route("ios-app-development-services/")]
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
        public ActionResult ReactNativeDevelopmentRedirect()
        {
            return RedirectToAction("ReactNativeDevelopment", "Avidclan");
        }

        [Route("react-native-app-development-company/")]
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
        public ActionResult XamarinAppDevelopmentRedirect()
        {
            return RedirectToAction("XamarinAppDevelopment", "Avidclan");
        }

        [Route("xamarin-development-company/")]
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
        public ActionResult GoogleCloudServicesRedirect()
        {
            return RedirectToAction("GoogleCloudServices", "Avidclan");
        }

        [Route("google-cloud-platform-services/")]
        public ActionResult GoogleCloudServices()
        {
            return View();
        }

        [Route("microsoft-azure-development/")]
        public ActionResult MicrosoftAzureDevelopmentRedirect()
        {
            return RedirectToAction("MicrosoftAzureDevelopment", "Avidclan");
        }

        [Route("microsoft-azure-devops/")]
        public ActionResult MicrosoftAzureDevelopment()
        {
            return View();
        }

        [Route("jenkins-management-services/")]
        public ActionResult JenkinsManagementServices()
        {
            //return View();
            return RedirectToAction("AvidclanAction", "Avidclan");
        }

        [Route("selenium-automation-testing-services/")]
        public ActionResult SeleniumAutomationTestingService()
        {
            return View();
        }

        [Route("microsoft-sql-server-manage/")]
        public ActionResult MicrosoftSqlServerManageRedirect()
        {
            return RedirectToAction("MicrosoftSqlServerManage", "Avidclan");
        }

        [Route("microsoft-sql-server-integration-services/")]
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
        public ActionResult PostgreSqlDevelopmentRedirect()
        {
            return RedirectToAction("PostgreSqlDevelopment", "Avidclan");
        }

        [Route("postgresql-database-management/")]
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
        public ActionResult MongodbDevelopmentRedirect()
        {
            return RedirectToAction("MongodbDevelopment", "Avidclan");
        }

        [Route("mongodb-development-services/")]
        public ActionResult MongodbDevelopment()
        {
            return View();
        }

        [Route("redis-development/")]
        public ActionResult RedisDevelopmentRedirect()
        {
            return RedirectToAction("RedisDevelopment", "Avidclan");
        }

        [Route("redis-database-development-company/")]
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
            //return View();
            return RedirectToAction("AvidclanAction", "Avidclan");
        }

        [Route("microsoft-dynamics-crm-development/")]
        public ActionResult MicrosoftDynamicsCRMDevelopment()
        {
            //return View();
            return RedirectToAction("AvidclanAction", "Avidclan");
        }

        [Route("sharepoint-development-company/")]
        public ActionResult SharePointDevelopment()
        {
            //return View();
            return RedirectToAction("AvidclanAction", "Avidclan");
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
                    var thumbnailImageString = BlogList[i].Image;

                    var thumbnailImageCDN = thumbnailImageString;
                    if (thumbnailImageString.Contains("https"))
                    {
                        thumbnailImageCDN = thumbnailImageString.Contains("localhost") ? thumbnailImageString : ImageServerUrl + thumbnailImageString;
                    }

                    Blog blog = new Blog();
                    blog.Title = BlogList[i].Title;
                    blog.Description = BlogList[i].Description;
                    blog.BlogType = BlogList[i].BlogType;
                    blog.Image = thumbnailImageCDN;
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
                    var thumbnailImageString = BlogList[i].Image;

                    var thumbnailImageCDN = thumbnailImageString;
                    if (thumbnailImageString.Contains("https"))
                    {
                        thumbnailImageCDN = thumbnailImageString.Contains("localhost") ? thumbnailImageString : ImageServerUrl + thumbnailImageString;
                    }

                    Blog blog = new Blog();
                    blog.Id = BlogList[i].Id;
                    blog.Title = BlogList[i].Title;
                    blog.Description = BlogList[i].Description;
                    blog.BlogType = BlogList[i].BlogType;
                    blog.Image = thumbnailImageCDN;
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
            }
            return JsonConvert.SerializeObject(new { Isvalid = true, data = obj });
        }

        public string GetRelatedPosts(int blogId)
        {
            int PageSize = 3;
            BlogViewModel obj = new BlogViewModel();
            List<Blog> listBlog = new List<Blog>();
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 12, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Id", blogId);
            parameters.Add("@Offset", 0);
            parameters.Add("@PagingSize", PageSize);
            var BlogList = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            if (BlogList != null && BlogList.Count > 0)
            {
                for (int i = 0; i < BlogList.Count; i++)
                {
                    var thumbnailImageString = BlogList[i].Image;

                    var thumbnailImageCDN = thumbnailImageString;
                    if (thumbnailImageString.Contains("https"))
                    {
                        thumbnailImageCDN = thumbnailImageString.Contains("localhost") ? thumbnailImageString : ImageServerUrl + thumbnailImageString;
                    }

                    Blog blog = new Blog();
                    blog.Id = BlogList[i].Id;
                    blog.Title = BlogList[i].Title;
                    blog.Description = BlogList[i].Description;
                    blog.BlogType = BlogList[i].BlogType;
                    blog.Image = thumbnailImageCDN;
                    blog.PostingDate = BlogList[i].PostingDate;
                    blog.PostedBy = BlogList[i].PostedBy;
                    blog.PageUrl = BlogList[i].PageUrl;
                    blog.MetaTitle = BlogList[i].MetaTitle;
                    blog.MetaDescription = BlogList[i].MetaDescription;
                    listBlog.Add(blog);
                }
            }
            return JsonConvert.SerializeObject(new { Isvalid = true, data = listBlog });
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
            if (bloglist != null && bloglist.Count > 0)
            {
                for (int i = 0; i < bloglist.Count; i++)
                {
                    var thumbnailImageString = bloglist[i].Image;

                    var thumbnailImageCDN = thumbnailImageString;
                    if (thumbnailImageString.Contains("https"))
                    {
                        thumbnailImageCDN = thumbnailImageString.Contains("localhost") ? thumbnailImageString : ImageServerUrl + thumbnailImageString;
                    }
                    bloglist[i].Image = thumbnailImageCDN;
                }

            }
            var dynamiclist = new
            {
                bloglist = bloglist,
                blogfaqslist = blogfaqslist
            };
            return Json(dynamiclist, JsonRequestBehavior.AllowGet);
        }


        [Route("flutter-app-development-company-india/")]
        public ActionResult FlutterAppDevelopmentINDIA()
        {
            return View();
        }
        [Route("ui-ux-design-company-usa/")]
        public ActionResult UiUxDesignCompanyUSA()
        {
            return View();
        }

        
    }
}