using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
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
using static System.Net.Mime.MediaTypeNames;

namespace Avidclan_BlogsVacancy.Controllers
{

    public class AdminController : ApiController
    {
        string senderEmail = "";
        string senderEmailPassword = "";
        string host = "";
        int port = 0;
        string receiverEmail = "";

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
                var BlogType = HttpContext.Current.Request["BlogType"];
                var PostingDate = HttpContext.Current.Request["PostingDate"];
                var PostedBy = HttpContext.Current.Request["PostedBy"];
                var Images = HttpContext.Current.Request.Files["Image"];
                var ImageUrls = HttpContext.Current.Request["ImageUrl"];
                var PageUrl = HttpContext.Current.Request["PageUrl"];
                var MetaTitle = HttpContext.Current.Request["MetaTitle"];
                var MetaDescription = HttpContext.Current.Request["MetaDescription"];
                var BlogDetailImage = HttpContext.Current.Request.Files["BlogDetailImage"];
                var BlogDetailImageString = HttpContext.Current.Request["BlogDetailImageUrl"];

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
                    System.IO.Stream fs = Images.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    ImageUrl = "data:image/png;base64," + base64String;
                    ImageName = Images.FileName;
                }
                if (BlogDetailImage != null && BlogDetailImageUrl == "")
                {
                    System.IO.Stream fs = BlogDetailImage.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    BlogDetailImageUrl = "data:image/png;base64," + base64String;
                    BlogDetailImageName = BlogDetailImage.FileName;
                }
                if (mode == 7)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@BlogId", BlogId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                    var DeleteBlogFaqs = con.Query<BlogFaqs>("sp_BlogFaqs", parameter, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                var parameters = new DynamicParameters();
                parameters.Add("@Id", BlogId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Title", Title, DbType.String, ParameterDirection.Input);
                parameters.Add("@Description", Description, DbType.String, ParameterDirection.Input);
                parameters.Add("@BlogType", BlogType, DbType.String, ParameterDirection.Input);
                parameters.Add("@Image", ImageUrl, DbType.String, ParameterDirection.Input);
                parameters.Add("@PostingDate", PostingDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@PostedBy", PostedBy, DbType.String, ParameterDirection.Input);
                parameters.Add("@ImageName", ImageName, DbType.String, ParameterDirection.Input);
                parameters.Add("@PageUrl", PageUrl, DbType.String, ParameterDirection.Input);
                parameters.Add("@MetaTitle", MetaTitle, DbType.String, ParameterDirection.Input);
                parameters.Add("@MetaDescription", MetaDescription, DbType.String, ParameterDirection.Input);
                parameters.Add("@BlogDetailImage", BlogDetailImageUrl, DbType.String, ParameterDirection.Input);
                parameters.Add("@BlogDetailImageName", BlogDetailImageName, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var SaveBlogDetails = connection.ExecuteScalar("sp_Blog", parameters, commandType: CommandType.StoredProcedure);
                    if(SaveBlogDetails != null)
                    {
                        SaveBlogFaqs(Faqarr, SaveBlogDetails);
                    }
                    else
                    {
                        SaveBlogDetails = BlogId;
                        SaveBlogFaqs(Faqarr, SaveBlogDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "";
        }

        public async void SaveImage(object id, HttpPostedFile file, string filename)
        {

            string path = System.Web.Hosting.HostingEnvironment.MapPath(String.Format("/BlogImages/Images/{0}", id));
            System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(path);
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                file.SaveAs(Path.Combine(path, filename));
            }
            else
            {
                foreach (FileInfo fileed in myDirInfo.GetFiles())
                {
                    fileed.Delete();
                    file.SaveAs(Path.Combine(path, file.FileName));
                }
            }
            string targetPath = @"/BlogImages/Images/" + id + "/" + filename;
            var Attribute = new DynamicParameters();
            Attribute.Add("@Id", id, DbType.String, ParameterDirection.Input);
            Attribute.Add("@Image", targetPath, DbType.String, ParameterDirection.Input);
            Attribute.Add("@mode", 2, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connections = new SqlConnection(connectionString))
            {
                var UpdateData = connections.ExecuteScalar("sp_Blog", Attribute, commandType: CommandType.StoredProcedure);
            }
        }

        [Route("api/Admin/SaveJobPosition")]
        [HttpPost]
        public async Task<string> SaveJobPosition(Careers data)
        {
            //  var userName = Request.Headers.GetCookies("EmailId").FirstOrDefault()?["EmailId"].Value;
            var userName = HttpContext.Current.Session["EmailId"];
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
                throw ex;
            }

            return "";
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
                    throw ex;
                }

            }
        }

        [Route("api/Admin/RequestForLeave")]
        [HttpPost]
        public async Task<string> RequestForLeave(LeaveViewModel leaveViewModel)
        {
            //SendLeaveMail(leaveViewModel.Leaves,leaveViewModel.ReportingPerson);
            var UserId = HttpContext.Current.Session["UserId"];
            var mode = 0;
            var ReportingPersons = string.Empty;
            if (leaveViewModel.ReportingPerson.Count > 0)
            {
                ReportingPersons = String.Join(",", leaveViewModel.ReportingPerson);
            }
           

            if (leaveViewModel.Id == 0)
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
                parameters.Add("@Id", leaveViewModel.Id, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@Fromdate", leaveViewModel.Fromdate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@Todate", leaveViewModel.Todate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                parameters.Add("@LeaveStatus", "Pending", DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", UserId, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@ReportingPerson", ReportingPersons, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", mode, DbType.Int32, ParameterDirection.Input);
                var SaveLeave = con.ExecuteScalar("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
                if (SaveLeave != null)
                {
                    leaveViewModel.Id = Convert.ToInt16(SaveLeave);
                }
                if (mode == 3)
                {
                    var parameter = new DynamicParameters();
                    parameter = new DynamicParameters();
                    parameter.Add("@LeaveId", leaveViewModel.Id, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
                    var deletelist = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure).ToList();
                }
                if (leaveViewModel.Leaves != null)
                {
                    foreach (var item in leaveViewModel.Leaves)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@LeaveDate", item.LeaveDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                        parameter.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                        parameter.Add("@LeaveId", leaveViewModel.Id, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                        var SaveLeaveDetails = con.ExecuteScalar("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                    }
                    var result = new LeaveController().CheckTypeOfLeave(leaveViewModel.Leaves, leaveViewModel.Fromdate, leaveViewModel.Id, UserId);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public void SendLeaveMail(List<LeaveDetailsViewModel> leaveDetailsViews,List<string> ReportingPerson)
        {
            var userName = HttpContext.Current.Session["EmailId"].ToString();
            ReadConfiguration();
            var mailbody = "<p>Respected Ma'am,<br>  I would like to request for leave for the following day(s).Hoping to receive a positive response from you.<br><br></p>" +
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
            "<p>Yours Sincerely,<br>" + userName + "</p></body></html>";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(receiverEmail);
                //mail.CC.Add("chintan.s@avidclan.com");
                //mail.CC.Add("rushil@avidclan.com");
                if(ReportingPerson.Count > 0)
                {
                    foreach(var person in ReportingPerson)
                    {
                        mail.CC.Add(person);
                    }
                }
                
                mail.From = new MailAddress(senderEmail);
                mail.Subject = "Leave Application";
                mail.Body = mailbody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("api/Admin/ReplyToLeaveRequest")]
        [HttpPost]
        public async Task<string> ReplyToLeaveRequest(LeaveViewModel leaveViewModel)
        {
            try
            {
                SendReplyForLeave(leaveViewModel.Leaves, leaveViewModel.LeaveStatus, leaveViewModel.FirstName, leaveViewModel.EmailId);
                var parameters = new DynamicParameters();
                parameters.Add("@LeaveStatus", leaveViewModel.LeaveStatus, DbType.String, ParameterDirection.Input);
                parameters.Add("@Id", leaveViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                var UpdateLeave = con.Query<LeaveViewModel>("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
                return "Reply Send Successfully";
            }
            catch (Exception ex)
            {
                var MessageException = ex.InnerException + ex.StackTrace;
                return MessageException;
            }

        }

        public void SendReplyForLeave(List<LeaveDetailsViewModel> leaveDetailsViews, string status, string name, string EmailId)
        {
            ReadConfiguration();
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
            "<p>Thanks & Regards,<br>XYZ</p></body></html>";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(EmailId);
                //mail.CC.Add("chintan.s@avidclan.com");
                //mail.CC.Add("rushil@avidclan.com");
                mail.From = new MailAddress(senderEmail);
                mail.Subject = "Reply For Leave Application";
                mail.Body = mailbody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveBlogFaqs(BlogFaqs[] blogFaqs, object id)
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
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        [Route("api/Admin/SaveFeedBack")]
        [HttpPost]
        public async Task<string> SaveFeedBack(FeedBack feedback)
        {
            //try
            //{
            //    var userId = HttpContext.Current.Session["UserId"];
            //    var parameters = new DynamicParameters();
            //    parameters.Add("@Id", feedback.Id, DbType.Int32, ParameterDirection.Input);
            //    parameters.Add("@Feedback", feedback.Feedback, DbType.String, ParameterDirection.Input);
            //    parameters.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);
            //    parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
            //    using (IDbConnection connection = new SqlConnection(connectionString))
            //    {
            //        var savefeedback = connection.ExecuteScalar("sp_FeedBack", parameters, commandType: CommandType.StoredProcedure);
            //        if(savefeedback != null)
            //        {
            //            return "FeedBack Saved Succesfully !";
            //        }
            //    }
            //    return "";
            //}
            //catch (Exception ex)
            //{
            //    string Error = ex.Message + ex.StackTrace;
            //    return Error;
            //}

            try
            {
                var userName = HttpContext.Current.Session["EmailId"].ToString();
                ReadConfiguration();
                MailMessage mail = new MailMessage();
                mail.To.Add(receiverEmail);
                //mail.CC.Add("chintan.s@avidclan.com");
                //mail.CC.Add("rushil@avidclan.com");
                mail.From = new MailAddress(senderEmail);
                mail.Subject = "FeedBack";
                mail.Body = feedback.Feedback;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtp.Send(mail);
                return "FeedBack Send Succesfully !";
            }
            catch (Exception ex)
            {
                string Error = ex.Message + ex.StackTrace;
                return Error;
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
