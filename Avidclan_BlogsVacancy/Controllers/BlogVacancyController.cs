using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avidclan_BlogsVacancy.Controllers
{
    public class BlogVacancyController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public BlogVacancyController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: BlogVacancy
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin userLogin)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EmailId", userLogin.EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", userLogin.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var logindata = con.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);
            if(logindata!= null)
            {
                //Session["EmailId"] = logindata;
                HttpCookie nameCookie = new HttpCookie("EmailId",logindata.ToString());
                nameCookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(nameCookie);
            }
            return Json(logindata,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Blog()
        {
            return View();
        }

        public ActionResult Careers()
        {
            return View();
        }

        public ActionResult GetBlogList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            var BlogList = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).ToList();
            return Json(BlogList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetJobPositionList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var JobPositionList = con.Query<Careers>("sp_Careers", parameters, commandType: CommandType.StoredProcedure).ToList();
            return Json(JobPositionList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCareer(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            var DeleteOption = con.Query<Careers>("sp_Careers", parameters, commandType: CommandType.StoredProcedure);
            return Json(DeleteOption, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCareerById(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
            var GetCareer = con.Query<Careers>("sp_Careers", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return Json(GetCareer, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBlog(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
            var DeleteBlog = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            //return Json(DeleteBlog, JsonRequestBehavior.AllowGet);
            return RedirectToAction("blog");
        }
        public JsonResult GetBlogById(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            var GetBlogDetails = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return Json(GetBlogDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public void BlogStatus(HeaderMenu menu)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MenuName", menu.MenuName, DbType.String, ParameterDirection.Input);
            parameters.Add("@Status", menu.Status, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var resultMenu = con.Query<HeaderMenu>("sp_TblMenu", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            //return Json(GetCareer, JsonRequestBehavior.AllowGet);
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