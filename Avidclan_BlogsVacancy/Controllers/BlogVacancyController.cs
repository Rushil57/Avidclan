using Avidclan_BlogsVacancy.Methods;
using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using iTextSharp.text.pdf.qrcode;
using MailKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Bcpg;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Avidclan_BlogsVacancy.Controllers
{
    public class BlogVacancyController : Controller
    {
        private static string thumbnailImageFolder = "Image";
        private static string blogDetailImagesFolder = "BlogDetailImages";
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
        public async Task<ActionResult> Login(UserLogin userLogin)
        {
            try
            {                
                var parameters = new DynamicParameters();
                parameters.Add("@EmailId", userLogin.EmailId, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                var logindata = con.Query<UserLogin>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (logindata != null)
                {
                    string enteredPassword = userLogin.Password;
                    string storedHash = logindata.Password;
                    try
                    {
                        bool isValid = PasswordHelper.VerifyPassword(enteredPassword, storedHash);
                        if (isValid)
                        {
                            Session["UserEmailId"] = logindata.EmailId;
                            Session["UserId"] = logindata.Id;
                            Session["UserJoiningDate"] = logindata.JoiningDate;
                            Session["UserProbationPeriod"] = logindata.ProbationPeriod;
                            Session["FirstName"] = logindata.FirstName;
                            Session["LastName"] = logindata.LastName;
                        }
                        else
                        {
                            logindata = null;
                            return Json(logindata, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception)
                    {
                        logindata = null;
                        return Json(logindata, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                return Json(logindata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("UserLogin", ex.Message, ex.StackTrace);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult UpdatePasswordById(UpdatePasswordModel userdata)
        {
            try
            {
                string passwordHash = PasswordHelper.HashPassword(userdata.Password);
                var Id = userdata.Id;
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@Password", passwordHash, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", 7, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var UpdatePassword = connection.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog("UpdatePasswordById", ex.Message, ex.StackTrace).RunSynchronously();
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }        

        public ActionResult Blog()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult Careers()
        {
            if (Session["UserEmailId"] == null)
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

            //Remove Existing Image..
            var Dparameters = new DynamicParameters();
            Dparameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            Dparameters.Add("@Mode", 11, DbType.Int32, ParameterDirection.Input);
            var blogDetails = con.Query<Blog>("sp_Blog", Dparameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

            if (!String.IsNullOrEmpty(blogDetails.ImageName))
            {
                int res = RemoveImage(blogDetails.ImageName, thumbnailImageFolder);
            }
            if (!String.IsNullOrEmpty(blogDetails.BlogDetailImageName))
            {
                int response = RemoveImage(blogDetails.BlogDetailImageName, blogDetailImagesFolder);
            }


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

        public int RemoveImage(string fileName, string folder)
        {
            try
            {
                var path = Path.Combine(Server.MapPath("~/" + folder), fileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return 1;
            }
            catch (Exception)
            {
                //ErrorLog("AdminController - RemoveImage", ex.Message, ex.StackTrace);
                return 0;
            }
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
            var jsonResult = Json(dynamiclist, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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
            Session["UserEmailId"] = null;
            Session["UserJoiningDate"] = null;
            Session["UserId"] = 0;
            Session["UserProbationPeriod"] = 0;
            Session["FirstName"] = null;
            Session["LastName"] = null;
        }

        public ActionResult LeaveStatus()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult WFHStatus()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult SalarySlips()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public string AddNewUser(UserRegister userRegister)
        {
            try
            {
                var mode = 0;
                if (userRegister.Id == 0)
                {
                    mode = 2;
                }
                else
                {
                    mode = 10;
                }
                if (userRegister.BreakMonth != null)
                {
                    // Extract the numeric part from BreakMonth
                    userRegister.BreakMonth = Regex.Match(userRegister.BreakMonth, @"\d+(\.\d+)?").Value;
                }
                //if (!userRegister.OnBreak)
                //{
                //    userRegister.BreakMonth = "0";
                //}                
                var parameters = new DynamicParameters();
                parameters.Add("@Id", userRegister.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FirstName", userRegister.FirstName, DbType.String, ParameterDirection.Input);
                parameters.Add("@LastName", userRegister.LastName, DbType.String, ParameterDirection.Input);
                parameters.Add("@EmailId", userRegister.EmailId, DbType.String, ParameterDirection.Input);                
                if(mode == 2)
                {
                    string passwordHash = PasswordHelper.HashPassword(userRegister.Password);
                    //parameters.Add("@Password", userRegister.Password, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Password", passwordHash, DbType.String, ParameterDirection.Input);
                }                
                parameters.Add("@PhoneNumber", userRegister.PhoneNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("@JoiningDate", userRegister.JoiningDate.ToShortDateString(), DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ProbationPeriod", userRegister.ProbationPeriod, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@OnBreak", userRegister.OnBreak, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@BreakMonth", userRegister.BreakMonth, DbType.String, ParameterDirection.Input);
                parameters.Add("@NoticePeriodDate", userRegister.NoticePeriodDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@LastWorkingDate", userRegister.LastWorkingDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var saveUserData = connection.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);
                    if (saveUserData != null)
                    {
                        SaveUserRole(saveUserData, userRegister.Role, false);
                    }
                    else
                    {
                        SaveUserRole(userRegister.Id, userRegister.Role, true);
                    }
                    //save employee is in notice period or not
                    //if(mode == 10)
                    //{
                    //    var noticeparameters = new DynamicParameters();
                    //    noticeparameters.Add("@UserId", userRegister.Id, DbType.Int32, ParameterDirection.Input);
                    //    noticeparameters.Add("@IsNoticePeriod", userRegister.IsNoticePeriod, DbType.Boolean, ParameterDirection.Input);
                    //    noticeparameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                    //    using (IDbConnection noticeconnection = new SqlConnection(connectionString))
                    //    {
                    //        var saveLeaves = noticeconnection.ExecuteScalar("sp_PastLeaves", noticeparameters, commandType: CommandType.StoredProcedure);
                    //    }
                    //}
                }
                return "Data Saved!";
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return message;
            }
        }
        public void SaveUserRole(object UserId, int RoleId, bool CheckUpdate)
        {
            var mode = 0;
            if (CheckUpdate)
            {
                mode = 11;
            }
            else
            {
                mode = 3;
            }
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RoleId", RoleId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
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

        public string SaveBalanaceLeave(BalanaceLeaveViewModel balanaceLeaveViewModel)
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
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                try
                {
                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        var saveLeaves = connection.ExecuteScalar("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure);
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return "Data Saved!";
            }
            return "No data found";
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
        public ActionResult RegisterEmployee()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult GetAllEmployees()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
            var EmployeeList = con.Query<UserRegister>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();
            return Json(EmployeeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeById(int Id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
            var EmployeeData = con.Query<UserRegister>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return Json(EmployeeData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteEmployee(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 12, DbType.Int32, ParameterDirection.Input);
            var DeleteBlog = con.Query<UserLogin>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return RedirectToAction("RegisterEmployee");
        }

        public ActionResult ReportingPerson()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult LeaveBalance()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        [HttpPost]
        public async Task<string> SaveReportingperson(string person, int Id)
        {
            var mode = 0;
            if (Id == 0)
            {
                mode = 8;
            }
            else
            {
                mode = 7;
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ReportingPersonEmail", person, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var saveUserData = await connection.ExecuteScalarAsync("sp_LeaveReportingPerson", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveReportingperson", ex.Message, ex.StackTrace);
                return ex.Message;
            }

            return "Data Saved!";
        }

        public ActionResult GetListOfReportingPerson()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            var ReportingpersonList = con.Query<ReportingPersons>("sp_LeaveReportingPerson", parameters, commandType: CommandType.StoredProcedure).ToList();
            return Json(ReportingpersonList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReportingPerson(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
            var Deleteperson = con.Query<UserLogin>("sp_LeaveReportingPerson", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return RedirectToAction("RegisterEmployee");
        }

        public ActionResult GetReportingPersonById(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            var EmployeeData = con.Query<ReportingPersons>("sp_LeaveReportingPerson", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return Json(EmployeeData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeeLeave()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult EmployeeReport()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public async Task<JsonResult> GetAllEmployeesLeaveDates()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
                var EmployeeData = con.Query<LeaveViewModel>("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure).ToList();
                return Json(EmployeeData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetAllEmployeesLeaveDates", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task<JsonResult> GetEmployeeLeaveReport(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 25, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@startdate", StartDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@enddate", EndDate, DbType.DateTime, ParameterDirection.Input);
                var EmployeeData = con.Query<LeaveViewModel>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure).ToList();
                if (EmployeeData.Count > 0)
                {
                    var groupedData = EmployeeData
                       .GroupBy(e => e.Id)
                       .Select(g => new
                       {
                           Id = g.Key,
                           Leaves = g
            .OrderByDescending(e =>
                (e.LeaveDate != DateTime.MinValue ? e.LeaveDate :
                 e.WFHDates != DateTime.MinValue ? e.WFHDates : DateTime.MinValue)
            )
            .ToList()
                       })
                       .ToList();
                    return Json(groupedData, JsonRequestBehavior.AllowGet);
                }
                return Json(EmployeeData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetAllEmployeesLeaveDates", ex.Message, ex.StackTrace);
                return null;
            }
        }


        public async Task<JsonResult> loadUserLeaveDetails(EmployeeReportViewModel employeeReportViewModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 26, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@startdate", employeeReportViewModel.StartDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@enddate", employeeReportViewModel.EndDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Id", employeeReportViewModel.UserId, DbType.Int32, ParameterDirection.Input);
                var EmployeeData = con.Query<LeaveViewModel>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure).ToList().OrderByDescending(x => x.LeaveDate);
                return Json(EmployeeData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetAllEmployeesLeaveDates", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task<JsonResult> loadUserWfhDetails(EmployeeReportViewModel employeeReportViewModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 27, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@startdate", employeeReportViewModel.StartDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@enddate", employeeReportViewModel.EndDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Id", employeeReportViewModel.UserId, DbType.Int32, ParameterDirection.Input);
                var EmployeeData = con.Query<LeaveViewModel>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure).ToList().OrderByDescending(x => x.WFHDates);
                return Json(EmployeeData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetAllEmployeesLeaveDates", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public ActionResult LeaveConvert()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }



        public ActionResult CompensationLeave()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult ReminderMail()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public ActionResult Holiday()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        public async Task<JsonResult> SaveHolidayDetails(HolidayViewModel holidayViewModel)
        {
            try
            {
                if (holidayViewModel.HolidayDate == null)
                    return Json(new { success = false, message = "Invalid date selected." });

                if (holidayViewModel.Id == 0) // only check on add
                {
                    var checkParams = new DynamicParameters();
                    checkParams.Add("@Mode", 9); // assuming mode 6 is a check mode
                    checkParams.Add("@Description", holidayViewModel.HolidayName, DbType.String);
                    checkParams.Add("@Date", holidayViewModel.HolidayFomattedDate, DbType.String);

                    var existing = 0;
                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        //await connection.ExecuteAsync("sp_Holidays", checkParams, commandType: CommandType.StoredProcedure);
                        existing = await connection.QueryFirstOrDefaultAsync<int>(
                                "sp_Holidays", checkParams, commandType: CommandType.StoredProcedure);
                    }

                    if (existing > 0)
                    {
                        return Json(new { success = false, message = "A holiday with the same name or date already exists." });
                    }
                }
                 var mode = 0;
                var successMessage = "";
                if (holidayViewModel.Id == 0)
                {
                    mode = 2;
                    successMessage = "Holiday saved successfully!";
                }
                else
                {
                    mode = 7;
                    successMessage = "Holiday Edited successfully!";
                }
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", mode, DbType.Int32);
                parameters.Add("@Description", holidayViewModel.HolidayName, DbType.String);
                parameters.Add("@Date", holidayViewModel.HolidayDate, DbType.String);
                parameters.Add("@Id", holidayViewModel.Id, DbType.Int32);

                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    await connection.ExecuteAsync("sp_Holidays", parameters, commandType: CommandType.StoredProcedure);
                }

                return Json(new { success = true, message = successMessage });
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - SaveHolidayDetails", ex.Message, ex.StackTrace);
                return Json(new { success = false, message = "An error occurred while saving the holiday." });
            }
        }


        public async Task<JsonResult> GetHolidayYear()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);

                var holidayYearList = con.Query<HolidayViewModel>("sp_Holidays", parameters, commandType: CommandType.StoredProcedure).ToList().OrderByDescending(x => x.HolidayYear);
                return Json(holidayYearList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetHolidayYear", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task<JsonResult> GetHolidayList(HolidayViewModel holidayViewModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Year", holidayViewModel.HolidayYear, DbType.Int32, ParameterDirection.Input);

                var holidayYearList = con.Query<HolidayViewModel>("sp_Holidays", parameters, commandType: CommandType.StoredProcedure).ToList();
                return Json(holidayYearList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetHolidayList", ex.Message, ex.StackTrace);
                return null;
            }
        }


        public async Task<JsonResult> GetMonthlyLeaves(EmployeeReportViewModel employeeReportViewModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@StartDate", employeeReportViewModel.StartDate, DbType.Date, ParameterDirection.Input);
                parameters.Add("@EndDate ", employeeReportViewModel.EndDate, DbType.Date, ParameterDirection.Input);

                var holidayYearList = con.Query<HolidayViewModel>("sp_Holidays", parameters, commandType: CommandType.StoredProcedure).ToList();
                return Json(holidayYearList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetMonthlyLeaves", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task<JsonResult> GetHolidayData(HolidayViewModel holidayViewModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Id", holidayViewModel.Id, DbType.Int32, ParameterDirection.Input);

                var holidaydata = con.Query<HolidayViewModel>("sp_Holidays", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return Json(holidaydata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetHolidayList", ex.Message, ex.StackTrace);
                return null;
            }
        }


        public async Task<JsonResult> DeleteHolidayData(HolidayViewModel holidayViewModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Id", holidayViewModel.Id, DbType.Int32, ParameterDirection.Input);

                var holidaydata = con.Query<HolidayViewModel>("sp_Holidays", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return Json(new { success = true, message = "Deleted Successfully!" });
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetHolidayList", ex.Message, ex.StackTrace);
                return null;
            }
        }

        
        public async Task<JsonResult> GetEmployeeStatusWise(HolidayViewModel holidayViewModel)
        {
            try
            {
                int status = Convert.ToInt16(holidayViewModel.Status);
                if(status == 0)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", 18, DbType.Int32, ParameterDirection.Input);
                    var inactiveuserData = con.Query<UserRegister>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();
                    return Json(new { success = true, data = inactiveuserData});
                }
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", 19, DbType.Int32, ParameterDirection.Input);
                    var activeuserData = con.Query<UserRegister>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();
                    return Json(new { success = true, data = activeuserData });
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("BlogVacancyController - GetEmployeeStatusWise", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task ErrorLog(string ControllerName, string ErrorMessage, string StackTrace)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ControllerName", ControllerName, DbType.String, ParameterDirection.Input);
            parameters.Add("@ErrorMessage", ErrorMessage, DbType.String, ParameterDirection.Input);
            parameters.Add("@StackTrace", StackTrace, DbType.String, ParameterDirection.Input);
            parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
            var SaveError = await con.ExecuteScalarAsync("sp_Errorlog", parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
