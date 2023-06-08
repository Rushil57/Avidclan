using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using iTextSharp.text.pdf.qrcode;
using Org.BouncyCastle.Bcpg;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

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

        public ActionResult UserRegister()
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
            var logindata = con.Query<UserLogin>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            if (logindata != null)
            {
                Session["EmailId"] = logindata.EmailId;
                Session["UserId"] = logindata.Id;
                Session["JoiningDate"] = logindata.JoiningDate;
                Session["ProbationPeriod"] = logindata.ProbationPeriod;
                Session["FirstName"] = logindata.FirstName;
            }
            return Json(logindata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Blog()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult Careers()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
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
            if (DeleteBlog == null)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@BlogId", id, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                var DeleteBlogFaqs = con.Query<BlogFaqs>("sp_BlogFaqs", parameter, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return RedirectToAction("blog");
        }
        public JsonResult GetBlogById(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            //var GetBlogDetails = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).ToList();
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

        public void Logout()
        {
            Session["EmailId"] = null;
            Session["JoiningDate"] = null;
            Session["UserId"] = 0;
            Session["ProbationPeriod"] = 0;
            Session["FirstName"] = null;
        }

        public ActionResult LeaveStatus()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult SalarySlips()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public bool AddNewUser(UserRegister userRegister)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", userRegister.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FirstName", userRegister.FirstName, DbType.String, ParameterDirection.Input);
                parameters.Add("@LastName", userRegister.LastName, DbType.String, ParameterDirection.Input);
                parameters.Add("@EmailId", userRegister.EmailId, DbType.String, ParameterDirection.Input);
                parameters.Add("@Password", userRegister.Password, DbType.String, ParameterDirection.Input);
                parameters.Add("@PhoneNumber", userRegister.PhoneNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("@JoiningDate", userRegister.JoiningDate.ToShortDateString(), DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ProbationPeriod", userRegister.ProbationPeriod, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@mode", 2, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var saveUserData = connection.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);
                    if (saveUserData != null)
                    {
                        SaveUserRole(saveUserData, userRegister.Role);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }



        public void SaveUserRole(object UserId, int RoleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RoleId", RoleId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@mode", 3, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var saveUserRole = connection.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);

            }
        }

        public string CheckEmailAdress(string Email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EmailId", Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@mode", 4, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var CheckEmailId = connection.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);
                if (CheckEmailId != null)
                {
                    return "Please Enter Unique Email Address";
                }
                return "";
            }
        }

        public JsonResult GetRoles()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
            var RoleList = con.Query<UserLogin>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();
            if (RoleList.Count > 0)
            {
                return Json(RoleList, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult BalanaceLeave()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public bool SaveBalanaceLeave(BalanaceLeaveViewModel balanaceLeaveViewModel)
        {
            if (balanaceLeaveViewModel != null)
            {
                var mode = 0;
                if (balanaceLeaveViewModel.Id != 0)
                {
                    mode = 8;
                }
                else
                {
                    mode = 4;
                }
               // IsNoticePeriod
                var parameters = new DynamicParameters();
                parameters.Add("@Id", balanaceLeaveViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@PersonalLeave", balanaceLeaveViewModel.PersonalLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@SickLeave", balanaceLeaveViewModel.SickLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", balanaceLeaveViewModel.UserId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@IsNoticePeriod", balanaceLeaveViewModel.NoticePeriod, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                try
                {
                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        var saveLeaves = connection.ExecuteScalar("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public JsonResult GetEmployeeList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
            var EmployeeList = con.Query<UserRegister>("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure).ToList();
            return Json(EmployeeList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetEmployees()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            var EmployeeList = con.Query<BalanaceLeaveViewModel>("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure).ToList();
            return Json(EmployeeList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetEmployeeListById(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
            var EmployeeList = con.Query<BalanaceLeaveViewModel>("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return Json(EmployeeList, JsonRequestBehavior.AllowGet);

        }
    }
}
