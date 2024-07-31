using Avidclan_BlogsVacancy.Methods;
using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using System.Xml.Linq;
using static iTextSharp.text.pdf.AcroFields;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Web.Configuration;
using SendGrid.Helpers.Mail.Model;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Avidclan_BlogsVacancy.Controllers
{

    public class AdminController : ApiController
    {
        string senderEmail = "";
        string senderEmailPassword = "";
        string host = "";
        int port = 0;
        string receiverEmail = "";

        private static string thumbnailImageFolder = "Image";
        private static string blogDetailImagesFolder = "BlogDetailImages";

        string ApiKey = WebConfigurationManager.AppSettings["SendGridMailApiKey"];
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public AdminController()
        {
            con = new SqlConnection(connectionString);
        }

        [Route("api/Admin/SaveBlog")]
        [HttpPost]
        public async Task<string> SaveBlog()
        {
            string ImageUrl = "";
            string BlogDetailImageUrl = string.Empty;
            string BlogDetailImageName = string.Empty;

            try
            {
                var Id = HttpContext.Current.Request["Id"];
                var Title = HttpContext.Current.Request["Title"];
                var Description = HttpContext.Current.Request["Description"];
                var ShortTitle = HttpContext.Current.Request["ShortTitle"];
                var BlogType = HttpContext.Current.Request["BlogType"];
                var PostingDt = HttpContext.Current.Request["PostingDate"].ToString();
                //DateTime PostingDt = DateTime.Parse(HttpContext.Current.Request["PostingDate"]);
                var PostedBy = HttpContext.Current.Request["PostedBy"];
                var Images = HttpContext.Current.Request.Files["Image"];
                var ImageUrls = HttpContext.Current.Request["ImageUrl"];
                var PageUrl = HttpContext.Current.Request["PageUrl"];
                var MetaTitle = HttpContext.Current.Request["MetaTitle"];
                var MetaDescription = HttpContext.Current.Request["MetaDescription"];
                var SchemaCode = HttpContext.Current.Request["SchemaCode"];
                var BlogDetailImage = HttpContext.Current.Request.Files["BlogDetailImage"];
                var BlogDetailImageString = HttpContext.Current.Request["BlogDetailImageUrl"];

                DateTime dt = DateTime.ParseExact(PostingDt, "MM/dd/yyyy HH:mm:ss", null);
                System.Data.SqlTypes.SqlDateTime PostingDate = System.Data.SqlTypes.SqlDateTime.Parse(dt.ToString("yyyy/MM/dd"));

                var Faqs = HttpContext.Current.Request["BlogFaqs"];

                BlogFaqs[] Faqarr = JsonConvert.DeserializeObject<BlogFaqs[]>(Faqs);
                var mode = 0;
                var BlogId = 0;
                string ImageName = "";
                if (Id == null || Id == "")
                {
                    mode = 1;
                }
                else
                {
                    mode = 7;
                    BlogId = Convert.ToInt32(Id);
                    ImageUrl = ImageUrls;
                    ImageName = Images.FileName;
                    BlogDetailImageUrl = BlogDetailImageString;
                }
                if (Images != null && ImageUrls == "")
                {
                    ImageName = Images.FileName;

                    //Remove existing blog
                    if (mode == 7)
                    {
                        var Dparameters = new DynamicParameters();
                        Dparameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
                        Dparameters.Add("@Mode", 11, DbType.Int32, ParameterDirection.Input);
                        var blogDetails = con.Query<Blog>("sp_Blog", Dparameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                        if (!String.IsNullOrEmpty(blogDetails.ImageName))
                        {
                            int res = await RemoveImage(blogDetails.ImageName, thumbnailImageFolder);
                        }
                    }

                    //Fetch the File.
                    HttpPostedFile thumbnailImage = HttpContext.Current.Request.Files["Image"];
                    var FilePath = await UploadImage(thumbnailImage, thumbnailImageFolder);
                    if (FilePath != null && FilePath != "")
                    {
                        ImageUrl = FilePath;

                    }
                }

                if (BlogDetailImage != null && BlogDetailImageUrl == "")
                {
                    BlogDetailImageName = BlogDetailImage.FileName;

                    if (mode == 7)
                    {
                        var Dparameters = new DynamicParameters();
                        Dparameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
                        Dparameters.Add("@Mode", 11, DbType.Int32, ParameterDirection.Input);
                        var blogDetails = con.Query<Blog>("sp_Blog", Dparameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                        if (!String.IsNullOrEmpty(blogDetails.BlogDetailImageName))
                        {
                            int res = await RemoveImage(blogDetails.BlogDetailImageName, blogDetailImagesFolder);
                        }
                    }

                    //Fetch the File.
                    HttpPostedFile BlogDetailsImage = HttpContext.Current.Request.Files["BlogDetailImage"];
                    var FilePath = await UploadImage(BlogDetailsImage, blogDetailImagesFolder);
                    if (FilePath != null && FilePath != "")
                    {
                        BlogDetailImageUrl = FilePath;
                    }

                }

                if (mode == 7)
                {
                    var Dynamicparameter = new DynamicParameters();
                    Dynamicparameter.Add("@BlogId", BlogId, DbType.Int32, ParameterDirection.Input);
                    Dynamicparameter.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                    var DeleteBlogFaqs = con.Query<BlogFaqs>("sp_BlogFaqs", Dynamicparameter, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                var parameters = new DynamicParameters();
                parameters.Add("@Id", BlogId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Title", Title, DbType.String, ParameterDirection.Input);
                parameters.Add("@Description", Description, DbType.String, ParameterDirection.Input);
                parameters.Add("@ShortTitle", ShortTitle, DbType.String, ParameterDirection.Input);
                parameters.Add("@BlogType", BlogType, DbType.String, ParameterDirection.Input);
                parameters.Add("@Image", ImageUrl, DbType.String, ParameterDirection.Input);
                parameters.Add("@PostingDate", PostingDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@PostedBy", PostedBy, DbType.String, ParameterDirection.Input);
                parameters.Add("@ImageName", ImageName, DbType.String, ParameterDirection.Input);
                parameters.Add("@PageUrl", PageUrl, DbType.String, ParameterDirection.Input);
                parameters.Add("@MetaTitle", MetaTitle, DbType.String, ParameterDirection.Input);
                parameters.Add("@MetaDescription", MetaDescription, DbType.String, ParameterDirection.Input);
                parameters.Add("@SchemaCode", SchemaCode, DbType.String, ParameterDirection.Input);
                parameters.Add("@BlogDetailImage", BlogDetailImageUrl, DbType.String, ParameterDirection.Input);
                parameters.Add("@BlogDetailImageName", BlogDetailImageName, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var SaveBlogDetails = connection.ExecuteScalar("sp_Blog", parameters, commandType: CommandType.StoredProcedure);
                    if (SaveBlogDetails != null)
                    {
                        await SaveBlogFaqs(Faqarr, SaveBlogDetails);
                    }
                    else
                    {
                        SaveBlogDetails = BlogId;
                        await SaveBlogFaqs(Faqarr, SaveBlogDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveBlog", ex.Message, ex.StackTrace);
            }
            return "";
        }

        [Route("api/Admin/SaveJobPosition")]
        [HttpPost]
        public async Task<string> SaveJobPosition(Careers data)
        {
            //  var userName = Request.Headers.GetCookies("EmailId").FirstOrDefault()?["EmailId"].Value;
            var userName = HttpContext.Current.Session["FirstName"].ToString();
            var mode = 0;
            var parameters = new DynamicParameters();
            if (data.Id == 0)
            {
                mode = 1;
                parameters.Add("@AddedBy", userName, DbType.String, ParameterDirection.Input);
            }
            else
            {
                mode = 6;
                parameters.Add("@UpdatedBy", userName, DbType.String, ParameterDirection.Input);
            }

            parameters.Add("@Id", data.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@JobTitle", data.JobTitle, DbType.String, ParameterDirection.Input);
            parameters.Add("@Experience", data.Experience, DbType.String, ParameterDirection.Input);
            parameters.Add("@Location", data.Location, DbType.String, ParameterDirection.Input);
            parameters.Add("@NoOfPosition", data.NoOfPosition, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Responsibilities", data.Responsibilities, DbType.String, ParameterDirection.Input);
            parameters.Add("@Qualification", data.Qualification, DbType.String, ParameterDirection.Input);
            parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
            try
            {
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var saveCareer = connection.ExecuteScalar("sp_Careers", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveJobPosition", ex.Message, ex.StackTrace);
            }

            return "";
        }

        public async Task<string> UploadImage(HttpPostedFile imageFile, string folder)
        {
            try
            {
                //Get Directory Path.
                string dirPath = HttpContext.Current.Server.MapPath("~/" + folder + "/");
                if (!Directory.Exists(dirPath))
                {
                    //Create directory if it does't exists..
                    Directory.CreateDirectory(dirPath);
                }

                //Fetch the Image File Name.
                string fileName = Path.GetFileName(imageFile.FileName);

                var filePath = Path.Combine(dirPath, fileName);

                //Save the File.
                imageFile.SaveAs(filePath);

                var imageUrl = Url.Content($"~/{folder}/{fileName}");
                return imageUrl;
            }catch(Exception ex)
            {
                ErrorLog("AdminController - UploadImage", ex.Message, ex.StackTrace);
                return "";
            }
        }

        public async Task<int> RemoveImage(string fileName, string folder)
        {
            try
            {
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/" + folder), fileName);

                if (System.IO.File.Exists(path))
                {
                    await ErrorLog("AdminController - RemoveImageExists", path, path);
                    System.IO.File.Delete(path);
                }
                return 1; // Redirect to an appropriate action
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - RemoveImage", ex.Message, ex.StackTrace);
                return 0;
            }
        }

        [Route("api/Admin/UpdateOrder")]
        [HttpPost]
        public async void UpdateOrder(List<UpdateOrder> updateOrder)
        {
            foreach (var item in updateOrder)
            {
                try
                {
                    var position = Convert.ToInt32(item.Order);
                    var parameters = new DynamicParameters();
                    parameters.Add("@JobTitle", item.Name, DbType.String, ParameterDirection.Input);
                    parameters.Add("@OrderPosition", position, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@mode", 7, DbType.Int32, ParameterDirection.Input);
                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        var GetOrder = connection.ExecuteScalar("sp_Careers", parameters, commandType: CommandType.StoredProcedure);
                    }
                }
                catch (Exception ex)
                {
                    await ErrorLog("AdminController - UpdateOrder", ex.Message, ex.StackTrace);
                }
            }
        }

        [Route("api/Admin/RequestForLeave")]
        [HttpPost]
        public async Task<string> RequestForLeave(LeaveViewModel leaveViewModel)
        {
            var UserId = HttpContext.Current.Session["UserId"];
            var FirstName = HttpContext.Current.Session["FirstName"].ToString();
            var LastName = HttpContext.Current.Session["LastName"].ToString();
            var JoinigDate = HttpContext.Current.Session["UserJoiningDate"];
            var ProbationPeriod = HttpContext.Current.Session["UserProbationPeriod"];

            List<LeaveViewModel> LeaveDataList = new List<LeaveViewModel>();

            try
            {
                var LeaveData = leaveViewModel.Leaves.Where(x => x.WorkAndHalfLeave == false && x.WorkFromHome == false).ToList();
                if (LeaveData.Count > 0)
                {
                    await GetConsecutiveLeaveDates(LeaveData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, false, JoinigDate, ProbationPeriod, UserId);
                }

                var WorkHomeData = leaveViewModel.Leaves.Where(x => x.WorkAndHalfLeave == false && x.WorkFromHome == true).ToList();
                if (WorkHomeData.Count > 0)
                {
                    await GetConsecutiveWorkFromHomeDates(WorkHomeData, leaveViewModel.ReasonForLeave, UserId);
                }

                var LeaveAndWfhData = leaveViewModel.Leaves.Where(x => x.WorkAndHalfLeave == true && x.WorkFromHome == true).ToList();
                if (LeaveAndWfhData.Count > 0)
                {
                    await GetConsecutiveWorkFromHomeDates(LeaveAndWfhData, leaveViewModel.ReasonForLeave, UserId);
                    await GetConsecutiveLeaveDates(LeaveAndWfhData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, true, JoinigDate, ProbationPeriod, UserId);
                }

                if (LeaveData.Count > 0 && WorkHomeData.Count == 0 && LeaveAndWfhData.Count == 0)
                {
                    await SendLeaveMail(LeaveData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, FirstName, LastName);
                }
                if (LeaveData.Count == 0 && WorkHomeData.Count > 0 && LeaveAndWfhData.Count == 0)
                {
                    await SendWorkFromHomeMail(WorkHomeData, leaveViewModel.ReportingPerson, leaveViewModel.ReasonForLeave, FirstName, LastName);
                    //await SendWorkFromHomeAndLeaveMail(WorkHomeData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson);
                }
                if (LeaveData.Count == 0 && WorkHomeData.Count == 0 && LeaveAndWfhData.Count > 0)
                {
                    await SendWorkFromHomeAndLeaveMail(LeaveAndWfhData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, FirstName, LastName);
                }
                if (LeaveData.Count == 0 && WorkHomeData.Count > 0 && LeaveAndWfhData.Count > 0)
                {
                    var leaveAndWfhData = WorkHomeData.Concat(LeaveAndWfhData).ToList();
                    await SendWorkFromHomeAndLeaveMail(leaveAndWfhData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, FirstName, LastName);
                }
                if (LeaveData.Count > 0 && WorkHomeData.Count > 0 && LeaveAndWfhData.Count == 0)
                {
                    var leaveAndWfhData = LeaveData.Concat(WorkHomeData).ToList();
                    await SendWorkFromHomeAndLeaveMail(leaveAndWfhData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, FirstName, LastName);
                }
                if (LeaveData.Count > 0 && WorkHomeData.Count == 0 && LeaveAndWfhData.Count > 0)
                {
                    var leaveAndWfhData = LeaveData.Concat(LeaveAndWfhData).ToList();
                    //leaveAndWfhData.Sort();
                    await SendWorkFromHomeAndLeaveMail(leaveAndWfhData, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, FirstName, LastName);
                }
                if (LeaveData.Count > 0 && WorkHomeData.Count > 0 && LeaveAndWfhData.Count > 0)
                {
                    await SendWorkFromHomeAndLeaveMail(leaveViewModel.Leaves, leaveViewModel.ReasonForLeave, leaveViewModel.ReportingPerson, FirstName, LastName);
                }
                return "Request Sent Successfully!";
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - RequestForLeave", ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public async Task SaveReportingPerson(List<string> ReportingPerson, object Leaveid)
        {
            try
            {
                foreach (var person in ReportingPerson)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@ReportingPerson", person, DbType.String, ParameterDirection.Input);
                    parameter.Add("@LeaveId", Leaveid, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                    var SaveReportingPerson = con.ExecuteScalar("sp_LeaveReportingPerson", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch(Exception ex)
            {
                await ErrorLog("AdminController - SaveReportingPerson", ex.Message, ex.StackTrace);
            }
            
        }
        public async Task SendLeaveMail(List<LeaveDetailsViewModel> leaveDetailsViews, string ReasonForLeave, List<string> ReportingPerson, string FirstName, string LastName)
        {

            await ReadConfiguration();
            try
            {
                var HalfDay = string.Empty;
			var Leave = string.Empty;

            if(leaveDetailsViews.Count == 1)
            {
				var checkHalfDay = leaveDetailsViews.All(item => item.Halfday is null);
				if (checkHalfDay == false)
				{
					HalfDay = " Half-Day leave";
                    Leave = "";
				}
                else
                {
                    HalfDay = "";
                    Leave = "leave";
                }
			}
            else
            {
				var result = leaveDetailsViews.All(item => item.Halfday is null);
				if (result == false)
				{
					Leave = "leave";
					HalfDay = " & Half-Day leave";
				}
                else
                {
					Leave = "leave";
					HalfDay = "";
				}
			}
			
			var mailbody = "<p>Hello Ma'am/Sir,<br><br>"+ FirstName +"&nbsp;"+ LastName +" would like to request "+ Leave + HalfDay + " for the following day(s).Hoping to receive a positive response from you.<br><b>Reason : </b>" + ReasonForLeave + "<br><br></p>" +
            "<html><body>" +
                "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                "<thead><tr style='background: #eee;'><th>Leave Date</th><th>Leave Day</th><th>Half Day</th></tr></thead>" +
                "<tbody class='leaveTable'>";
            foreach (var leavedetails in leaveDetailsViews)
            {
                string WeekDay = leavedetails.LeaveDate.DayOfWeek.ToString();
                var addrow = "<tr><td style='background: #fff;'>" + leavedetails.LeaveDate.ToShortDateString() + "</td><td>" + WeekDay + "</td><td>" + leavedetails.Halfday + "</td></tr>";
                mailbody += addrow;
            }
            mailbody += "</tbody></table><br><br>" +
            "<p>Yours Sincerely,<br>" + FirstName + "&nbsp;" + LastName + "</p></body></html>";
            
                var subject = "Leave Application";
                await sendEmail(senderEmail, senderEmail, FirstName + "  " + LastName, subject, mailbody, ReportingPerson);

            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendLeaveMail", ex.Message, ex.StackTrace);
            }
        }

        [Route("api/Admin/ReplyToLeaveRequest")]
        [HttpPost]
        public async Task<string> ReplyToLeaveRequest(LeaveViewModel leaveViewModel)
        {
            try
            {
               await SendReplyForLeave(leaveViewModel.Leaves, leaveViewModel.LeaveStatus, leaveViewModel.FirstName, leaveViewModel.EmailId, leaveViewModel.Id);
                var parameters = new DynamicParameters();
                parameters.Add("@LeaveStatus", leaveViewModel.LeaveStatus, DbType.String, ParameterDirection.Input);
                parameters.Add("@Id", leaveViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                var UpdateLeave = con.Query<LeaveViewModel>("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
                return "Reply Send Successfully";
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - ReplyToLeaveRequest", ex.Message, ex.StackTrace);
                var MessageException = ex.InnerException + ex.StackTrace;
                return MessageException;
            }

        }

        public async Task SendReplyForLeave(List<LeaveDetailsViewModel> leaveDetailsViews, string status, string name, string EmailId,object Id)
        {
            await ReadConfiguration();
            try
            {
                List<ReportingPersons> ReportingPerson = new List<ReportingPersons>();
            ReportingPerson = GetReportingPerson(Id);
            var mailbody = "<p>Hello " + name + "<br>  Your leave has been<b> " + status + " </b>for the following day(s).<br><br></p>" +
            "<html><body>" +
                "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                "<thead><tr style='background: #eee;'><th>Leave Date</th><th>Leave Day</th><th>Half Day</th></tr></thead>" +
                "<tbody class='leaveTable'>";
            foreach (var leavedetails in leaveDetailsViews)
            {
                string WeekDay = leavedetails.LeaveDate.DayOfWeek.ToString();
                var addrow = "<tr><td style='background: #fff;'>" + leavedetails.LeaveDate.ToShortDateString() + "</td><td>" + WeekDay + "</td><td>" + leavedetails.Halfday + "</td></tr>";
                mailbody += addrow;
            }
            mailbody += "</tbody></table><br><br>" +
            "<p>Thanks & Regards,<br>HR, Avidclan</p></body></html>";
            
                var subject = "Reply For Leave Application";

                List<string> reportingpersonList = new List<string>();
                if (ReportingPerson != null)
                {
                    if (ReportingPerson.Count > 0)
                    {
                        foreach (var person in ReportingPerson)
                        {
                            reportingpersonList.Add(person.ReportingPerson);
                        }
                    }
                }

                await sendEmail(senderEmail, EmailId, name, subject, mailbody, reportingpersonList);
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendReplyForLeave", ex.Message, ex.StackTrace);
            }
        }

        public List<ReportingPersons> GetReportingPerson(object Id)
        {
            List<ReportingPersons> ReportingPerson = new List<ReportingPersons>();
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveId", Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode",2, DbType.Int32, ParameterDirection.Input);
            ReportingPerson = con.Query<ReportingPersons>("sp_LeaveReportingPerson", parameters, commandType: CommandType.StoredProcedure).ToList();
            return ReportingPerson;
        }
        public async Task SaveBlogFaqs(BlogFaqs[] blogFaqs, object id)
        {
            try
            {
                if (blogFaqs.Length > 0)
                {
                    foreach (var blog in blogFaqs)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@Questions", blog.Questions, DbType.String, ParameterDirection.Input);
                        parameters.Add("@Answers", blog.Answers, DbType.String, ParameterDirection.Input);
                        parameters.Add("@BlogId", id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                        var SaveBlogDetails = con.ExecuteScalar("sp_BlogFaqs", parameters, commandType: CommandType.StoredProcedure);

                    }
                }
            }
            catch(Exception ex)
            {
                await ErrorLog("AdminController - SaveBlogFaqs", ex.Message, ex.StackTrace);
            }
        }
        public async Task<bool> ReadConfiguration()
        {
            var result = false;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                var resultConfiguration = con.Query<EMailConfiguration>("sp_EmailConfiguration", parameters, commandType: CommandType.StoredProcedure).ToList().FirstOrDefault();
                if (resultConfiguration != null)
                {
                    senderEmail = resultConfiguration.FromMail;
                    senderEmailPassword = resultConfiguration.Password;
                    host = resultConfiguration.Host;
                    port = Convert.ToInt32(resultConfiguration.Port);
                    receiverEmail = resultConfiguration.ToMail;
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - ReadConfiguration", ex.Message, ex.StackTrace);
            }
            return result;
        }

        [Route("api/Admin/SaveFeedBack")]
        [HttpPost]
        public async Task<string> SaveFeedBack(FeedBack feedback)
        {
            try
            {
                var FirstName = HttpContext.Current.Session["FirstName"].ToString();
                var LastName = HttpContext.Current.Session["FirstName"].ToString();

                await ReadConfiguration();
                var subject = "FeedBack For Avidclan Technology";

                List<string> reportingPerson = new List<string>();
                await sendEmail(senderEmail, senderEmail, FirstName + "&nbsp;" + LastName, subject, feedback.Feedback, reportingPerson);
                return "FeedBack Send Succesfully !";
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveFeedBack", ex.Message, ex.StackTrace);
                string Error = ex.Message + ex.StackTrace;
                return Error;
            }
        }

        public async Task SendWorkFromHomeMail(List<LeaveDetailsViewModel> leaveDetailsViews, List<string> ReportingPerson, string ReasonForLeave, string FirstName, string LastName)
        {
            await ReadConfiguration();
            try
            {

            var mailbody = "<p>Hello Ma'am/Sir,<br><br>" + FirstName + "&nbsp;" + LastName  +" request work from home for following day(s). Hoping to receive a positive response from you.<br><b>Reason : </b>" + ReasonForLeave + "<br><br></p>" +
            "<html><body>" +
                "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                "<thead><tr style='background: #eee;'><th>Work From Home Date</th><th>Work From Home Day</th><th>Half Day</th></tr></thead>" +
                "<tbody class='leaveTable'>";
            foreach (var leavedetails in leaveDetailsViews)
            {
                string WeekDay = leavedetails.LeaveDate.DayOfWeek.ToString();
                var addrow = "<tr><td style='background: #fff;'>" + leavedetails.LeaveDate.ToShortDateString() + "</td><td>" + WeekDay + "</td><td>" + leavedetails.Halfday + "</td></tr>";
                mailbody += addrow;
            }
            mailbody += "</tbody></table><br><br>" +
            "<p>Yours Sincerely,<br>" + FirstName + "&nbsp;" + LastName + "</p></body></html>";
                var subject = "Request For Work From Home";
                await sendEmail(senderEmail, senderEmail, FirstName + "  " + LastName, subject, mailbody, ReportingPerson);
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendWorkFromHomeMail", ex.Message, ex.StackTrace);
            }
        }
        public async Task GetConsecutiveLeaveDates(List<LeaveDetailsViewModel> range, string ReasonForLeave, List<string> ReportingPerson, bool checkLeaveAndWfh, object JoiningDate, object ProbationPeriod, object UserId)
        {
            var startDate = range.FirstOrDefault();
            DateTime dateTime = startDate.LeaveDate;
            try
            {
                List<LeaveDetailsViewModel> LeaveDataList = new List<LeaveDetailsViewModel>();
                List<LeaveDetailsViewModel> leaveDataDetails = new List<LeaveDetailsViewModel>();
                var wantedDate = startDate.LeaveDate;

                foreach (var day in range.OrderBy(d => d.LeaveDate))
                {
                    LeaveDetailsViewModel leaveViewDetails = new LeaveDetailsViewModel();
                    leaveViewDetails.Fromdate = day.LeaveDate;
                    leaveViewDetails.Todate = day.LeaveDate;
                    leaveViewDetails.Halfday = day.Halfday;
                    leaveViewDetails.WorkFromHome = day.WorkFromHome;
                    leaveViewDetails.WorkAndHalfLeave = day.WorkAndHalfLeave;
                    leaveDataDetails.Add(leaveViewDetails);

                    if (day.LeaveDate != wantedDate || LeaveDataList.Count == 0)
                    {
                        LeaveDetailsViewModel leaveView = new LeaveDetailsViewModel();
                        leaveView.Id = day.Id;
                        leaveView.Fromdate = day.LeaveDate;
                        leaveView.Todate = day.LeaveDate;
						leaveView.Halfday = day.Halfday;
						leaveView.WorkFromHome = day.WorkFromHome;
						leaveView.WorkAndHalfLeave = day.WorkAndHalfLeave;
						LeaveDataList.Add(leaveView);
                    }
                    else
                    {
                        var lastRecord = LeaveDataList.LastOrDefault();
                        lastRecord.Todate = day.LeaveDate;
                    }
                    wantedDate = day.LeaveDate.AddDays(1);

                }
                foreach (var leaveData in LeaveDataList)
                {
                    var list = leaveDataDetails.Where(x => x.Fromdate >= leaveData.Fromdate && x.Fromdate <= leaveData.Todate).ToList();
                    await SaveLeaveApplicationData(leaveData.Id, leaveData.Fromdate, leaveData.Todate, ReasonForLeave, list, ReportingPerson, checkLeaveAndWfh, UserId, JoiningDate, ProbationPeriod);
                }
            }
            catch(Exception ex)
            {
                await ErrorLog("AdminController - GetConsecutiveLeaveDates", ex.Message, ex.StackTrace);
            }
          
        }
        public async Task SaveLeaveApplicationData(int Id, DateTime FromDate, DateTime ToDate, string ReasonForLeave, List<LeaveDetailsViewModel> leaveDataDetails, List<string> ReportingPerson, bool checkLeaveAndWfh, object UserId, object JoiningDate, object ProbationPeriod)
        {

            var mode = 0;
            if (Id == 0)
            {
                mode = 1;
            }
            else
            {
                mode = 3;
            }
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@Fromdate", FromDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@Todate", ToDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@LeaveStatus", "Pending", DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", UserId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@LeaveReason", ReasonForLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                var SaveLeave = await con.ExecuteScalarAsync("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
                if (SaveLeave != null)
                {
                    if (ReportingPerson != null)
                    {
                        if (ReportingPerson.Count > 0)
                        {
                            await SaveReportingPerson(ReportingPerson, SaveLeave);
                        }
                    }
                    //leaveViewModel[0].Id = Convert.ToInt16(SaveLeave);
                    await SaveLeaveApplicationDetailsData(leaveDataDetails, SaveLeave, UserId, checkLeaveAndWfh, JoiningDate, ProbationPeriod);
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveLeaveApplicationData", ex.Message, ex.StackTrace);
            }

        }
        public async Task SaveLeaveApplicationDetailsData(List<LeaveDetailsViewModel> leaveViewModel, object Leaveid, object UserId, bool checkLeaveAndWfh, object JoinigDate, object ProbationPeriod)
        {
            try
            {
                var GetFromDate = leaveViewModel.FirstOrDefault();
            var mode = 0;
            if (Leaveid != null)
            {
                mode = 1;
            }
            else
            {
                mode = 3;
            }
            
                if (mode == 3)
                {
                    var parameter = new DynamicParameters();
                    parameter = new DynamicParameters();
                    parameter.Add("@LeaveId", Leaveid, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
                    var deletelist = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure).ToList();
                }
                if (leaveViewModel != null)
                {
                    foreach (var item in leaveViewModel)
                    {
                        if (checkLeaveAndWfh)
                        {
                            if (item.Halfday == "SecondHalf")
                            {
                                item.Halfday = "FirstHalf";
                            }
                            else
                            {
                                item.Halfday = "SecondHalf";
                            }
                        }

                        var parameter = new DynamicParameters();
                        parameter.Add("@LeaveDate", item.Fromdate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                        parameter.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                        parameter.Add("@LeaveId", Leaveid, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                        var SaveLeaveDetails = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);

						if (checkLeaveAndWfh)
						{
						    if (item.Halfday == "SecondHalf")
						    {
						        item.Halfday = "FirstHalf";
						    }
						    else
						    {
						        item.Halfday = "SecondHalf";
						    }
						}
					}
					var result = await new LeaveController().CheckTypeOfLeave(
                            leaveViewModel, GetFromDate.Fromdate,
                            Leaveid, UserId, JoinigDate, ProbationPeriod);
                }

            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveLeaveApplicationDetailsData", ex.Message, ex.StackTrace);
            }
        }

        public async Task GetConsecutiveWorkFromHomeDates(List<LeaveDetailsViewModel> range, string ReasonForLeave, object UserId)
        {
            var startDate = range.FirstOrDefault();
            DateTime dateTime = startDate.LeaveDate;

            try
            {
                List<LeaveDetailsViewModel> LeaveDataList = new List<LeaveDetailsViewModel>();
                List<LeaveDetailsViewModel> leaveDataDetails = new List<LeaveDetailsViewModel>();
                var wantedDate = startDate.LeaveDate;

                foreach (var day in range.OrderBy(d => d.LeaveDate))
                {
                    LeaveDetailsViewModel leaveViewDetails = new LeaveDetailsViewModel();
                    leaveViewDetails.Fromdate = day.LeaveDate;
                    leaveViewDetails.Todate = day.LeaveDate;
                    leaveViewDetails.Halfday = day.Halfday;
                    leaveViewDetails.WorkFromHome = day.WorkFromHome;
                    leaveViewDetails.WorkAndHalfLeave = day.WorkAndHalfLeave;
                    leaveDataDetails.Add(leaveViewDetails);

                    if (day.LeaveDate != wantedDate || LeaveDataList.Count == 0)
                    {
                        LeaveDetailsViewModel leaveView = new LeaveDetailsViewModel();
                        leaveView.Id = day.Id;
                        leaveView.Fromdate = day.LeaveDate;
                        leaveView.Todate = day.LeaveDate;
                        leaveView.Halfday = day.Halfday;
                        leaveView.WorkFromHome = day.WorkFromHome;
                        leaveView.WorkAndHalfLeave = day.WorkAndHalfLeave;
                        LeaveDataList.Add(leaveView);
                    }
                    else
                    {
                        var lastRecord = LeaveDataList.LastOrDefault();
                        lastRecord.Todate = day.LeaveDate;
                    }
                    wantedDate = day.LeaveDate.AddDays(1);
                }
                foreach (var leaveData in LeaveDataList)
                {
                    var list = leaveDataDetails.Where(x => x.Fromdate >= leaveData.Fromdate && x.Fromdate <= leaveData.Todate).ToList();
                    await SaveworkFromData(list, ReasonForLeave, leaveData.Id, leaveData.Fromdate, leaveData.Todate, UserId);

                }
            }
            catch(Exception ex)
            {
                await ErrorLog("AdminController - GetConsecutiveWorkFromHomeDates", ex.Message, ex.StackTrace);
            }
           
        }
        public async Task SaveworkFromData(List<LeaveDetailsViewModel> leaveViewModel, string ReasonForLeave, object Id, DateTime FromDate, DateTime ToDate, object UserId)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@Fromdate", FromDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@Todate", ToDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@WFHStatus", "Pending", DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", UserId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@WFHReason", ReasonForLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                var SaveWFH = con.ExecuteScalar("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure);
               
                if (leaveViewModel != null)
                {
                    foreach (var item in leaveViewModel)
                    {
                        //if (checkLeaveAndWfh)
                        //{
                        //    if (item.Halfday == "SecondHalf")
                        //    {
                        //        item.Halfday = "FirstHalf";
                        //    }
                        //    else
                        //    {
                        //        item.Halfday = "SecondHalf";
                        //    }
                        //}
                        var parameter = new DynamicParameters();
                        parameter.Add("@WFHDates", item.Fromdate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                        parameter.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                        parameter.Add("@WFHId", SaveWFH, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                        var SaveLeaveDetails = con.ExecuteScalar("sp_WorkFromHomeDetails", parameter, commandType: CommandType.StoredProcedure);

                        //if (checkLeaveAndWfh)
                        //{
                        //    if (item.Halfday == "SecondHalf")
                        //    {
                        //        item.Halfday = "FirstHalf";
                        //    }
                        //    else
                        //    {
                        //        item.Halfday = "SecondHalf";
                        //    }
                        //}


                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveworkFromData", ex.Message, ex.StackTrace);
            }
        }


        public async Task SendWorkFromHomeAndLeaveMail(List<LeaveDetailsViewModel> leaveDetailsViews, string ReasonForLeave, List<string> ReportingPerson, string FirstName, string LastName)
        {

            await ReadConfiguration();
            try
            {
                var mailbody = "<p>Hello Ma'am/Sir,<br><br>" + FirstName + "&nbsp;" + LastName +" would like to request leave & Work From Home for the following day(s). Hoping to receive a positive response from you.<br><b>Reason : </b>" + ReasonForLeave + "<br><br></p>" +
            "<html><body>" +
                "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
				"<thead><tr style='background: #eee;'><th>Date</th><th>Day</th><th>Work From Home</th><th>WFH Half Day</th><th></th></tr></thead>" +
                "<tbody class='leaveTable'>";
            foreach (var leavedetails in leaveDetailsViews)
            {
                var WorkfromHome = string.Empty;
                var Leave = string.Empty;
                var WFHHalfDay = string.Empty;
                var LeaveHalfDay = string.Empty;

                if (leavedetails.WorkFromHome == true && leavedetails.WorkAndHalfLeave == false)
                {
                    if(leavedetails.Halfday != null)
                    {
                        WFHHalfDay = leavedetails.Halfday;
						if (leavedetails.Halfday == "FirstHalf")
						{
							LeaveHalfDay = "Second Half WFO";
						}
						else
						{
							LeaveHalfDay = "First Half WFO";
						}
					}
                    WorkfromHome = "Yes";
                }
                if(leavedetails.WorkFromHome == true && leavedetails.WorkAndHalfLeave == true && leavedetails.Halfday != null)
                {
                    Leave = "Yes";
					WorkfromHome = "Yes";
                    if(leavedetails.Halfday == "FirstHalf")
                    {
						WFHHalfDay = leavedetails.Halfday;
                        LeaveHalfDay = "Second Half Leave";
					}
                    else
                    {
						WFHHalfDay = leavedetails.Halfday;
						LeaveHalfDay = "First Half Leave";
					}
				}
                if(leavedetails.WorkFromHome == false && leavedetails.WorkAndHalfLeave == false)
                {
                    if (leavedetails.Halfday != null)
                    {
                        LeaveHalfDay = leavedetails.Halfday + " Leave";
                    }
                    else
                    {
                        LeaveHalfDay = "Full Day Leave";

					}
                }
                string WeekDay = leavedetails.LeaveDate.DayOfWeek.ToString();
                var addrow = "<tr><td style='background: #fff;'>" + leavedetails.LeaveDate.ToShortDateString() + "</td><td>" + WeekDay + "</td><td>" + WorkfromHome + "</td><td>" + WFHHalfDay + "</td><td>" + LeaveHalfDay + "</td></tr>";
                mailbody += addrow;
            }
            mailbody += "</tbody></table><br><br>" +
            "<p>Yours Sincerely,<br>" + FirstName + "&nbsp;" + LastName + "</p></body></html>";
            
                var subject = "Request For Leave and WFH";
                await sendEmail(senderEmail, senderEmail, FirstName + "  " + LastName, subject, mailbody, ReportingPerson);
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendWorkFromHomeAndLeaveMail", ex.Message, ex.StackTrace);
            }
        }

        public async Task ErrorLog(string ControllerName, string ErrorMessage, string StackTrace)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ControllerName", ControllerName, DbType.String, ParameterDirection.Input);
            parameters.Add("@ErrorMessage", ErrorMessage, DbType.String, ParameterDirection.Input);
            parameters.Add("@StackTrace", StackTrace, DbType.String, ParameterDirection.Input);
            parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
            var SaveError = con.ExecuteScalar("sp_Errorlog", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task sendEmail(string fromEmail, string toEmail, string reciverName, string subject, string message, List<string> ReportingPerson)
        {
            string apiKey = "a101ac38119207e6774e78a74701c990";
            string apiSecret = "fc46b6850d50f957b087e2ba1bf2c0ee";
            string apiUrl = "https://api.mailjet.com/v3.1/send";
            using (HttpClient client = new HttpClient())
            {
                string base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

                var ccList = new List<object>();
                ReportingPerson.Add("rushil@avidclan.com");
                ReportingPerson.Add("chintan.s@avidclan.com");
                if (ReportingPerson.Count > 0)
                {
                    foreach (var ccEmail in ReportingPerson)
                    {
                        ccList.Add(new { Email = ccEmail, Name = "" });
                    }
                }

                var emailPayload = new
                {
                    Messages = new[]
                    {
                  new
                  {
                      From = new { Email = fromEmail, Name = reciverName },
                      To = new[] { new { Email = toEmail, Name = "Avidclan Technologies" } },
                      CC = ccList.ToArray(),
                      Subject = subject,
                      HTMLPart = message
                  }
              }
                };

                string jsonPayload = JsonConvert.SerializeObject(emailPayload);
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Email sent successfully!");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
            }
        }

    }

    public class UpdateOrder
    {
        public string Name { get; set; }
        public string Order { get; set; }
    }

    public class FeedBack
    {
        public int Id { get; set; }
        public string Feedback { get; set; }
        public int UserId { get; set; }
    }
    
}
