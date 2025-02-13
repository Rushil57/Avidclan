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
using System.Text.RegularExpressions;

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
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - UploadImage", ex.Message, ex.StackTrace);
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
                int leaveId = 0;
                var LeaveData = leaveViewModel.Leaves.Where(x => x.WorkAndHalfLeave == false && x.WorkFromHome == false).ToList();
                if (LeaveData.Count > 0)
                {
                    leaveId = await GetConsecutiveLeaveDates(LeaveData, leaveViewModel, false, JoinigDate, ProbationPeriod, UserId, leaveViewModel.WFHId, leaveId);
                }

                var LeaveAndWfhData = leaveViewModel.Leaves.Where(x => x.WorkAndHalfLeave == true && x.WorkFromHome == true).ToList();
                if (LeaveAndWfhData.Count > 0)
                {
                    var updatedleaveId = leaveId;
                    leaveId = await GetConsecutiveLeaveDates(LeaveAndWfhData, leaveViewModel, true, JoinigDate, ProbationPeriod, UserId, leaveViewModel.WFHId, leaveId);
                    var tempLeaveid = leaveId == 0 ? leaveId : updatedleaveId;
                    await GetConsecutiveWorkFromHomeDates(LeaveAndWfhData, leaveViewModel, UserId, tempLeaveid, leaveViewModel.WFHId);
                }

                var WorkHomeData = leaveViewModel.Leaves.Where(x => x.WorkAndHalfLeave == false && x.WorkFromHome == true).ToList();
                if (WorkHomeData.Count > 0)
                {
                    await GetConsecutiveWorkFromHomeDates(WorkHomeData, leaveViewModel, UserId, leaveId, leaveViewModel.WFHId);
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

        public async Task SaveReportingPerson(List<string> ReportingPerson, object Leaveid, object wfhId)
        {
            try
            {
                foreach (var person in ReportingPerson)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@ReportingPerson", person, DbType.String, ParameterDirection.Input);
                    parameter.Add("@LeaveId", Leaveid, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@wfhId", wfhId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                    var SaveReportingPerson = con.ExecuteScalar("sp_LeaveReportingPerson", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
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

                if (leaveDetailsViews.Count == 1)
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

                var mailbody = "<p>Hello Ma'am/Sir,<br><br>" + FirstName + "&nbsp;" + LastName + " would like to request " + Leave + HalfDay + " for the following day(s).Hoping to receive a positive response from you.<br><b>Reason : </b>" + ReasonForLeave + "<br><br></p>" +
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

        public async Task SendReplyForLeave(List<LeaveDetailsViewModel> leaveDetailsViews, string status, string name, string EmailId, object Id)
        {
            await ReadConfiguration();
            try
            {
                List<ReportingPersons> ReportingPerson = new List<ReportingPersons>();
                ReportingPerson = GetReportingPerson(Id, "Leave");
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

        public List<ReportingPersons> GetReportingPerson(object Id, string status)
        {
            List<ReportingPersons> ReportingPerson = new List<ReportingPersons>();
            var parameters = new DynamicParameters();
            parameters.Add(status == "WFH" ? "@WfhId" : "@LeaveId", Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", status == "WFH" ? 9 : 2, DbType.Int32, ParameterDirection.Input);
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
            catch (Exception ex)
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

                var mailbody = "<p>Hello Ma'am/Sir,<br><br>" + FirstName + "&nbsp;" + LastName + " request work from home for following day(s). Hoping to receive a positive response from you.<br><b>Reason : </b>" + ReasonForLeave + "<br><br></p>" +
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
        public async Task<int> GetConsecutiveLeaveDates(List<LeaveDetailsViewModel> range, LeaveViewModel leaveViewModel, bool checkLeaveAndWfh, object JoiningDate, object ProbationPeriod, object UserId, object wfhId, int leaveId)
        {
            var leaveApplicationId = 0;
            var fromDate = range.FirstOrDefault();
            var ToDate = range.LastOrDefault();
            try
            {
                leaveApplicationId = await SaveLeaveApplicationData(leaveViewModel.Id, fromDate.LeaveDate, ToDate.LeaveDate, leaveViewModel.ReasonForLeave, range, leaveViewModel.ReportingPerson, checkLeaveAndWfh, UserId, JoiningDate, ProbationPeriod, wfhId, leaveId);
                return leaveApplicationId;
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - GetConsecutiveLeaveDates", ex.Message, ex.StackTrace);
                return 0;
            }



        }
        public async Task<int> SaveLeaveApplicationData(int Id, DateTime FromDate, DateTime ToDate, string ReasonForLeave, List<LeaveDetailsViewModel> leaveDataDetails, List<string> ReportingPerson, bool checkLeaveAndWfh, object UserId, object JoiningDate, object ProbationPeriod, object wfhId, int leaveId)
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
                            await SaveReportingPerson(ReportingPerson, SaveLeave, 0);
                        }
                    }
                    //leaveViewModel[0].Id = Convert.ToInt16(SaveLeave);
                    var tempLeaveId = leaveId == 0 ? SaveLeave : leaveId;
                    await SaveLeaveApplicationDetailsData(leaveDataDetails, tempLeaveId, UserId, checkLeaveAndWfh, JoiningDate, ProbationPeriod, "Added", wfhId);
                    //var LeaveId = Convert.ToInt32(SaveLeave);
                    return Convert.ToInt32(tempLeaveId);
                }
                else
                {
                    var tempLeaveId = leaveId == 0 ? Id : leaveId;
                    await SaveLeaveApplicationDetailsData(leaveDataDetails, tempLeaveId, UserId, checkLeaveAndWfh, JoiningDate, ProbationPeriod, "Edited", wfhId);
                    return tempLeaveId;
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveLeaveApplicationData", ex.Message, ex.StackTrace);
                return 0;
            }

        }
        public async Task SaveLeaveApplicationDetailsData(List<LeaveDetailsViewModel> leaveViewModel, object Leaveid, object UserId, bool checkLeaveAndWfh, object JoinigDate, object ProbationPeriod, string newlyAddedOrEdit, object wfhId)
        {
            try
            {
                var GetFromDate = leaveViewModel.FirstOrDefault();
                var mode = 0;
                if (newlyAddedOrEdit == "Added")
                {
                    mode = 1;
                }
                else
                {
                    mode = 3;
                }

                //if (mode == 3)
                //{
                //    var parameter = new DynamicParameters();
                //    parameter = new DynamicParameters();
                //    parameter.Add("@LeaveId", Leaveid, DbType.Int32, ParameterDirection.Input);
                //    parameter.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
                //    var deletelist = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure).ToList();
                //}
                if (leaveViewModel != null)
                {
                    if (mode == 1)
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
                            parameter.Add("@LeaveDate", item.LeaveDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
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
                                leaveViewModel, GetFromDate.LeaveDate,
                                Leaveid, UserId, JoinigDate, ProbationPeriod, wfhId);
                    }
                    if (mode == 3)
                    {
                        var leaveParameters = new DynamicParameters();
                        leaveParameters.Add("@LeaveId", Leaveid, DbType.Int16, ParameterDirection.Input);
                        leaveParameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);

                        var leaveDates = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", leaveParameters, commandType: CommandType.StoredProcedure).ToList();

                        foreach (var item in leaveViewModel)
                        {
                            var LeaveBalance = leaveDates
                                    .Where(x => x.LeaveDate == item.LeaveDate)
                                    .Select(x => new { x.PersonalLeaves, x.SickLeaves })
                                    .FirstOrDefault();

                            if (item.Halfday != null)
                            {
                                if (LeaveBalance.PersonalLeaves != null)
                                {
                                    item.PersonalLeaves = "0.5";
                                }
                                if (LeaveBalance.SickLeaves != null)
                                {
                                    item.SickLeaves = "0.5";
                                }
                            }
                            else
                            {
                                item.PersonalLeaves = LeaveBalance.PersonalLeaves;
                                item.SickLeaves = LeaveBalance.SickLeaves;
                            }
                            if (checkLeaveAndWfh && item.Halfday != null)
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

                            // Check if leaveViewModel contains a matching record based on properties
                            var recordExists = leaveDates.FirstOrDefault(l => l.LeaveDate == item.LeaveDate);
                            if (recordExists != null)
                            {
                                var parameter = new DynamicParameters();
                                parameter.Add("@LeaveDate", item.LeaveDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                                parameter.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                                parameter.Add("@PersonalLeaves", item.PersonalLeaves, DbType.String, ParameterDirection.Input);
                                parameter.Add("@SickLeaves", item.SickLeaves, DbType.String, ParameterDirection.Input);
                                parameter.Add("@Id", recordExists.Id, DbType.String, ParameterDirection.Input);
                                parameter.Add("@mode", 16, DbType.Int32, ParameterDirection.Input);
                                var SaveLeaveDetails = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                            }
                            else
                            {

                                var parameter = new DynamicParameters();
                                parameter.Add("@LeaveDate", item.LeaveDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                                parameter.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                                parameter.Add("@LeaveId", Leaveid, DbType.Int64, ParameterDirection.Input);
                                parameter.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                                parameter.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                                var SaveLeaveDetails = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveLeaveApplicationDetailsData", ex.Message, ex.StackTrace);
            }
        }

        public async Task GetConsecutiveWorkFromHomeDates(List<LeaveDetailsViewModel> range, LeaveViewModel leaveViewModel, object UserId, int leaveId, object wfhId)
        {
            var startDate = range.FirstOrDefault();
            var toDate = range.LastOrDefault();

            try
            {
                await SaveworkFromData(range, leaveViewModel, startDate.LeaveDate, toDate.LeaveDate, UserId, leaveId, wfhId);

            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - GetConsecutiveWorkFromHomeDates", ex.Message, ex.StackTrace);
            }

        }
        public async Task SaveworkFromData(
                    List<LeaveDetailsViewModel> leaveViewModel,
                    LeaveViewModel leaveView,
                    DateTime fromDate,
                    DateTime toDate,
                    object userId,
                    int leaveId,
                    object wfhId)
        {
            try
            {
                int mode = (Convert.ToInt32(wfhId) != 0) ? 8 : 1;
                if (mode == 8) leaveView.Id = Convert.ToInt32(wfhId);


                var parameters = new DynamicParameters();
                parameters.Add("@Id", leaveView.Id, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@Fromdate", fromDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@Todate", toDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@WFHStatus", "Pending", DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", userId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@WFHReason", leaveView.ReasonForLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@LeaveId", leaveId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                var saveWFH = con.ExecuteScalar("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure);
                if (saveWFH != null)
                {
                    if (leaveView.ReportingPerson != null)
                    {
                        if (leaveView.ReportingPerson.Count > 0)
                        {
                            await SaveReportingPerson(leaveView.ReportingPerson, 0, saveWFH);
                        }
                    }
                }
                if (leaveViewModel != null)
                {
                    //if (mode == 8) await DeleteLeaveDetailsAsync(wfhId, 9);

                    foreach (var item in leaveViewModel)
                    {
                        var existingWFH = GetExistingWFH(item.LeaveDate, userId);
                        saveWFH = saveWFH == null ? wfhId : saveWFH;
                        if (existingWFH == null)
                        {
                            await SaveWFHDetails(item, saveWFH);
                        }
                        else
                        {
                            await DeleteWfhDetailsAsync(saveWFH, 6);
                        }

                        //if (mode == 8)
                        //{
                        if (string.IsNullOrEmpty(item.Halfday))
                        {
                            var leaveDates = GetLeaveDates(leaveId);
                            var record = leaveDates?.FirstOrDefault(l => l.LeaveDate == item.LeaveDate);
                            if (record != null) await DeleteLeaveDetailsAsync(record.Id, 15);
                        }

                        if (item.WorkFromHome && !item.WorkAndHalfLeave)
                        {
                            var leaveDetail = GetExistingLeaveDetail(item.LeaveDate, userId);
                            if (leaveDetail != null) await DeleteLeaveDetailsAsync(leaveDetail.Id, 15);
                        }
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SaveworkFromData", ex.Message, ex.StackTrace);
            }
        }

        //private DynamicParameters CreateParameters(object paramObj)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@Id", id, DbType.Int64, ParameterDirection.Input);
        //    parameters.Add("@Fromdate", fromDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
        //    parameters.Add("@Todate", toDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
        //    parameters.Add("@WFHStatus", "Pending", DbType.String, ParameterDirection.Input);
        //    parameters.Add("@UserId", userId, DbType.Int16, ParameterDirection.Input);
        //    parameters.Add("@WFHReason", reasonForLeave, DbType.String, ParameterDirection.Input);
        //    parameters.Add("@LeaveId", leaveId, DbType.Int16, ParameterDirection.Input);
        //    parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
        //    var saveWFH = con.ExecuteScalar("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure);
        //}


        private async Task DeleteLeaveDetailsAsync(object id, int mode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
            await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
        }

        private async Task DeleteWfhDetailsAsync(object id, int mode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
            await con.ExecuteScalarAsync("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure);
        }

        private LeaveDetailsViewModel GetExistingWFH(DateTime fromDate, object userId)
        {
            //var parameters = CreateParameters(new { FromDate = fromDate.ToShortDateString(), UserId = userId, Mode = 5 });
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", fromDate.ToShortDateString(), DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@mode", 5, DbType.Int32, ParameterDirection.Input);
            return con.Query<LeaveDetailsViewModel>("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        private async Task SaveWFHDetails(LeaveDetailsViewModel item, object saveWFH)
        {
            //var parameters = CreateParameters(new
            //{
            //    WFHDates = item.Fromdate.ToShortDateString(),
            //    Halfday = item.Halfday,
            //    WFHId = saveWFH,
            //    Mode = 1
            //});
            var parameters = new DynamicParameters();
            parameters.Add("@WFHDates", item.LeaveDate.ToShortDateString(), DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
            parameters.Add("@WFHId", saveWFH, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
            await con.ExecuteScalarAsync("sp_WorkFromHomeDetails", parameters, commandType: CommandType.StoredProcedure);
        }

        private List<LeaveDetailsViewModel> GetLeaveDates(int leaveId)
        {
            //var parameters = CreateParameters(new { LeaveId = leaveId, Mode = 8 });
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveId", leaveId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@mode", 8, DbType.Int32, ParameterDirection.Input);
            return con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure).ToList();
        }

        private LeaveDetailsViewModel GetExistingLeaveDetail(DateTime leaveDate, object userId)
        {
            //var parameters = CreateParameters(new { LeaveDate = leaveDate.ToShortDateString(), UserId = userId, Mode = 18 });
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveDate", leaveDate.ToShortDateString(), DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@mode", 18, DbType.Int32, ParameterDirection.Input);
            return con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }



        public async Task SendWorkFromHomeAndLeaveMail(List<LeaveDetailsViewModel> leaveDetailsViews, string ReasonForLeave, List<string> ReportingPerson, string FirstName, string LastName)
        {

            await ReadConfiguration();
            try
            {
                var mailbody = "<p>Hello Ma'am/Sir,<br><br>" + FirstName + "&nbsp;" + LastName + " would like to request leave & Work From Home for the following day(s). Hoping to receive a positive response from you.<br><b>Reason : </b>" + ReasonForLeave + "<br><br></p>" +
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
                        if (leavedetails.Halfday != null)
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
                    if (leavedetails.WorkFromHome == true && leavedetails.WorkAndHalfLeave == true && leavedetails.Halfday != null)
                    {
                        Leave = "Yes";
                        WorkfromHome = "Yes";
                        if (leavedetails.Halfday == "FirstHalf")
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
                    if (leavedetails.WorkFromHome == false && leavedetails.WorkAndHalfLeave == false)
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

        //work from home reply

        [Route("api/Admin/ReplyToWFHRequest")]
        [HttpPost]
        public async Task<string> ReplyToWFHRequest(LeaveViewModel leaveViewModel)
        {
            try
            {
                //await SendReplyForWFH(leaveViewModel.WFH, leaveViewModel.WFHStatus, leaveViewModel.FirstName, leaveViewModel.EmailId, leaveViewModel.Id);
                await SendReplyForWFH(leaveViewModel);
                var parameters = new DynamicParameters();
                parameters.Add("@WFHStatus", leaveViewModel.WFHStatus, DbType.String, ParameterDirection.Input);
                parameters.Add("@Id", leaveViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                var UpdateLeave = con.Query<LeaveViewModel>("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure);
                return "Reply Send Successfully";
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - ReplyToWFHRequest", ex.Message, ex.StackTrace);
                var MessageException = ex.InnerException + ex.StackTrace;
                return MessageException;
            }

        }

        public async Task SendReplyForWFH(LeaveViewModel leaveViewModel)
        {
            await ReadConfiguration();
            try
            {
                List<ReportingPersons> ReportingPerson = new List<ReportingPersons>();
                ReportingPerson = GetReportingPerson(leaveViewModel.Id, "WFH");
                var mailbody = "<p>Hello " + leaveViewModel.FirstName + "<br>  Your work for home has been<b> " + leaveViewModel.WFHStatus + " </b>for the following day(s).<br><br></p>" +
                "<html><body>" +
                    "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                    "<thead><tr style='background: #eee;'><th>WFH Date</th><th>WFH Day</th><th>Half Day</th></tr></thead>" +
                    "<tbody class='leaveTable'>";
                foreach (var wfhdetail in leaveViewModel.WFH)
                {
                    string WeekDay = wfhdetail.WFHDate.DayOfWeek.ToString();
                    var addrow = "<tr><td style='background: #fff;'>" + wfhdetail.WFHDate.ToShortDateString() + "</td><td>" + WeekDay + "</td><td>" + wfhdetail.HalfDay + "</td></tr>";
                    mailbody += addrow;
                }
                mailbody += "</tbody></table><br><br>" +
                "<p>Thanks & Regards,<br>HR, Avidclan</p></body></html>";

                var subject = "Reply For Work From Home Application";

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

                await sendEmail(senderEmail, leaveViewModel.EmailId, leaveViewModel.FirstName, subject, mailbody, reportingpersonList);
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendReplyForWFH", ex.Message, ex.StackTrace);
            }
        }


        [Route("api/Admin/AdminLeaveRequest")]
        [HttpPost]
        public async Task<string> AdminLeaveRequest(LeaveViewModel leaveViewModel)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Fromdate", leaveViewModel.Fromdate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@Todate", leaveViewModel.Todate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@LeaveStatus", "Approved", DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", leaveViewModel.UserId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@LeaveReason", leaveViewModel.ReasonForLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                var SaveLeave = await con.ExecuteScalarAsync("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
                if (SaveLeave != null)
                {
                    if (leaveViewModel.ReportingPerson != null)
                    {
                        if (leaveViewModel.ReportingPerson.Count > 0)
                        {
                            await SaveReportingPerson(leaveViewModel.ReportingPerson, SaveLeave, 0);
                        }
                    }

                    // Get user leave balance
                    var userParams = new DynamicParameters();
                    userParams.Add("@Id", leaveViewModel.UserId, DbType.String, ParameterDirection.Input);
                    userParams.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);

                    var leaveBalance = con.Query<UserRegister>("sp_User", userParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    double userTotalPaidLeave = string.IsNullOrEmpty(leaveBalance?.PaidLeave) ? 0 : Convert.ToDouble(leaveBalance.PaidLeave);
                    double userTotalSickLeave = string.IsNullOrEmpty(leaveBalance?.SickLeave) ? 0 : Convert.ToDouble(leaveBalance.SickLeave);

                    double totalPlUsed = 0;
                    foreach (var item in leaveViewModel.Leaves)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@LeaveDate", item.LeaveDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                        parameter.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                        parameter.Add("@LeaveId", SaveLeave, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@UserId", leaveViewModel.UserId, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                        var SaveLeaveDetails = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                        if(SaveLeaveDetails != null)
                        {
                            double full_Day_pl = 1;
                            if (item.Halfday != null)
                            {
                                full_Day_pl = 0.5;
                                totalPlUsed += 0.5;
                            }
                            else
                            {
                                totalPlUsed++;
                            }


                            var PLparameters = new DynamicParameters();
                            PLparameters.Add("@PersonalLeaves", full_Day_pl, DbType.Decimal, ParameterDirection.Input);
                            PLparameters.Add("@Id", SaveLeaveDetails, DbType.Int64, ParameterDirection.Input);
                            PLparameters.Add("@PastLeave", 0, DbType.Decimal, ParameterDirection.Input);
                            PLparameters.Add("@mode", 9 , DbType.Int32, ParameterDirection.Input);
                            await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", PLparameters, commandType: CommandType.StoredProcedure);

                        }

                    }
                    if (leaveBalance != null)
                    {
                        var remainingPl = Math.Max(0, userTotalPaidLeave - totalPlUsed);
                        await new LeaveController().UpdateLeaveBalance(leaveViewModel.UserId, remainingPl,userTotalSickLeave);
                    }
                }

                var Emailparameters = new DynamicParameters();
                Emailparameters.Add("@Id", leaveViewModel.UserId, DbType.Int32, ParameterDirection.Input);
                Emailparameters.Add("@mode", 16, DbType.Int32, ParameterDirection.Input);
                var userEmailAddress = con.Query<UserRegister>("sp_User", Emailparameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                await SendMailForLeaveByAdmin(leaveViewModel.Leaves, userEmailAddress, SaveLeave);

                return "Leave Updated Successfully!";
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - AdminLeaveRequest", ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public async Task SendMailForLeaveByAdmin(List<LeaveDetailsViewModel> leaveViewModel, UserRegister userDetails,object leaveId)
        {
            await ReadConfiguration();
            try
            {
                List<ReportingPersons> ReportingPerson = new List<ReportingPersons>();
                ReportingPerson = GetReportingPerson(leaveId, "Leave");
                var mailbody = "<p>Hello " + userDetails.FirstName + "<br>  Your request for Converting Sick Leaves to Paid Leaves is Successfully Implemeted for the following day(s).<br><br></p>" +
                "<html><body>" +
                    "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                    "<thead><tr style='background: #eee;'><th>WFH Date</th><th>WFH Day</th><th>Half Day</th></tr></thead>" +
                    "<tbody class='leaveTable'>";
                foreach (var leaveDetail in leaveViewModel)
                {
                    string WeekDay = leaveDetail.LeaveDate.DayOfWeek.ToString();
                    var addrow = "<tr><td style='background: #fff;'>" + leaveDetail.LeaveDate.ToShortDateString() + "</td><td>" + WeekDay + "</td><td>" + leaveDetail.Halfday + "</td></tr>";
                    mailbody += addrow;
                }
                mailbody += "</tbody></table><br><br>" +
                "<p>Thanks & Regards,<br>HR, Avidclan</p></body></html>";

                var subject = "Coverting Sick Leaves to Paid Leaves";

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

                await sendEmail(senderEmail, userDetails.EmailId, userDetails.FirstName, subject, mailbody, reportingpersonList);
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendReplyForWFH", ex.Message, ex.StackTrace);
            }
        }

        [Route("api/Admin/SendCompOffRequest")]
        [HttpPost]
        public async Task<string> SendCompOffRequest(CompOffViewModel compOffViewModel)
        {
            try
            {
                var userIdObj = HttpContext.Current.Session["UserId"];
                if (userIdObj == null)
                {
                    return "User is not logged in.";
                }
                Match match = Regex.Match(compOffViewModel.NumberOfDays, @"\d+(\.\d+)?"); // Extracts only numbers
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userIdObj, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@NumberOfDays", match.Value, DbType.String, ParameterDirection.Input);
                parameters.Add("@Reason", compOffViewModel.Reason, DbType.String, ParameterDirection.Input);
                parameters.Add("@Status", "Pending", DbType.String, ParameterDirection.Input);
                parameters.Add("@Mode", 1, DbType.Int64, ParameterDirection.Input);
                var SaveLeave = await con.ExecuteScalarAsync("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure);

                await SendMailForCompOffRequest(compOffViewModel, userIdObj);

                return "Request sent successfully! After approval it will be added to your leave balanace!";
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - AdminLeaveRequest", ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public async Task SendMailForCompOffRequest(CompOffViewModel compOffViewModel,object UserId)
        {
            await ReadConfiguration();
            try
            {
                var Emailparameters = new DynamicParameters();
                Emailparameters.Add("@Id", UserId, DbType.Int32, ParameterDirection.Input);
                Emailparameters.Add("@mode", 16, DbType.Int32, ParameterDirection.Input);
                var userDetails = con.Query<UserRegister>("sp_User", Emailparameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                // Ensure userDetails is not null
                if (userDetails == null)
                {
                    throw new Exception("User details not found.");
                }

                var mailbody = $@"
                    <html>
                    <body>
                        <p>Hello Ma'am/Sir,</p>
                        <p><strong>{userDetails.FirstName} {userDetails.LastName}</strong> would like to request Compensation Leave.</p>
                        <p><b>Reason:</b> {compOffViewModel.Reason}</p>
                        <p><b>Number of Days:</b> {compOffViewModel.NumberOfDays}</p>
                        <p>Hoping to receive a positive response from you.</p>
                        <br>
                        <p>Yours sincerely,</p>
                        <p>{userDetails.FirstName} {userDetails.LastName}</p>
                    </body>
                    </html>";

                var subject = "Request for Compensation Leave";
                await sendEmail(senderEmail, senderEmail, $"{userDetails.FirstName} {userDetails.LastName}", subject, mailbody, null);
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendMailForCompOffRequest", ex.Message, ex.StackTrace);
            }
        }

        
        [Route("api/Admin/ReplyToCompOffRequest")]
        [HttpPost]
        public async Task<string> ReplyToCompOffRequest(CompOffViewModel compOffViewModel)
        {
            try
            {
                await SendReplyForCompOff(compOffViewModel);
                var parameters = new DynamicParameters();
                parameters.Add("@Status", compOffViewModel.Status, DbType.String, ParameterDirection.Input);
                parameters.Add("@Id", compOffViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                var UpdateStatus = con.Query<CompOffViewModel>("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure);
                return "Reply Send Successfully";
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - ReplyToCompOffRequest", ex.Message, ex.StackTrace);
                var MessageException = ex.InnerException + ex.StackTrace;
                return MessageException;
            }

        }

        public async Task SendReplyForCompOff(CompOffViewModel compOffViewModel)
        {
            await ReadConfiguration();
            try
            {
                string firstName = compOffViewModel.FirstName;
                string status = compOffViewModel.Status;
                string reason = compOffViewModel.Reason;
                string noOfdays = compOffViewModel.NumberOfDays;

                var mailbody = $@"
                <html>
                <body>
                    <p>Hello {firstName},</p>
                    <p>Your Compensation Leave Request has been <b>{status}</b>.</p>
                    <p><b>Reason : </b>{reason}.</p>
                    <p><b>Number of Days : </b>{noOfdays}.</p>
                    <br>
                    <p>Thanks & Regards,</p>
                    <p><b>HR, Avidclan</b></p>
                </body>
                </html>";

                var subject = "Reply For Compensation Leave Request";

                List<string> reportingpersonList = new List<string>();

                 await sendEmail(senderEmail, compOffViewModel.EmailId, firstName, subject, mailbody, reportingpersonList);
            }
            catch (Exception ex)
            {
                await ErrorLog("AdminController - SendReplyForCompOff", ex.Message, ex.StackTrace);
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

                // Ensure ReportingPerson is not null
                if (ReportingPerson == null)
                {
                    ReportingPerson = new List<string>();
                }
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
