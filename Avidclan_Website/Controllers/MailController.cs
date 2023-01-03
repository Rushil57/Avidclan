using Antlr.Runtime;
using Avidclan_Website.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel;
using System.Configuration;
using System.Web.Services.Description;
using Dapper;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
//using System.Net.Mail;

namespace Avidclan_Website.Controllers
{
    public class MailController : ApiController
    {
        //string senderEmail = ConfigurationManager.AppSettings["FromEmail"];
        //string senderEmailPassword = ConfigurationManager.AppSettings["FromPassword"];
        //string host = ConfigurationManager.AppSettings["Host"];
        //int port = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
        //string receiverEmail = ConfigurationManager.AppSettings["ToEmail"];
        string senderEmail = "";
        string senderEmailPassword = "";
        string host = "";
        int port = 0;
        string receiverEmail = "";

        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public MailController()
        {
            con = new SqlConnection(connectionString);
        }

        [Route("api/Mail/SendContactDetails")]
        [HttpPost]
        public async Task<string> SendContactDetails(UserDetail obj)
        {
            try
            {
                await ReadConfiguration();
                var messagebody = "<html><body>" +
                                    "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                                    "<tr style='background: #eee;'><td colspan='2'><strong>Contact Inquiry Details</strong></td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>FirstName:</strong> </td><td>" + obj.FirstName + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>LastName:</strong> </td><td>" + obj.LastName + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Email:</strong> </td><td>" + obj.Email + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Mobile/Phone:</strong> </td><td>" + "+" + obj.CountryCode + "&nbsp;" + obj.Phoneumber + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Comment/Message:</strong> </td><td>" + obj.Message + " </td></tr>" +
                                   "</table>" +
                                   "</body></html>";

                MailMessage mail = new MailMessage();
                mail.To.Add(receiverEmail);
                mail.From = new MailAddress(senderEmail);
                mail.Subject = "Contact Inquiry From Avidclan Technologies";
                mail.Body = messagebody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
                throw ex.InnerException;
            }
            return "Sent";
        }

        [Route("api/Mail/SendProjectDetails")]
        [HttpPost]
        public async Task<string> SendProjectDetails(ProjectDetail projectDetail)
        {
            await ReadConfiguration();
            var messagebody = "<html>" +
                                 "<body style='font-family: lato, Helvetica, sans-serif;font-size: 16px;width:600px;'>" +
                                     "<div style='padding: 15px 30px;background: #1d5fa5;color: #fff;'>" +
                                         "<h4 style='padding: 0;margin: 0;'>Project Inquiry Details</h4>" +
                                      "</div>" +
                                        "<div style='padding: 15px 30px;background: #f8f8f8;'>" +
                                             "<p>Hello,<br/>" + projectDetail.ProjectDetails + "<br/><br/>" +
                                             "<strong>Service Name:</strong>" + projectDetail.Service + "<br/><br/>" +
                                             "<strong>Budget:</strong>" + projectDetail.Budget + "<br/><br/>" +
                                             "<strong>Start Time:</strong>" + projectDetail.StartDate + "<br/><br/>" +
                                             "<strong>Project Requirement:</strong>" + projectDetail.Requirement + "<br/><br/>" +
                                             "Regards,<br/><br/><strong>" + projectDetail.FirstName + "&nbsp;" + projectDetail.LastName + "</strong><br/>" +
                                              projectDetail.Email + "<br/>" + "+" + projectDetail.CountryCode + "&nbsp;" + projectDetail.Phone +
                                              "<br/><br/><br/>This mail is sent via project form on Avidclan Technologies Web Site <br/>" +
                                              "<a href='https://www.avidclan.com'>https://www.avidclan.com</a>" +
                                         "</p></div>" +
                                 "</body></html>";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(receiverEmail);
                mail.From = new MailAddress(senderEmail);
                mail.Subject = "Project Inquiry From Avidclan Technologies";
                mail.Body = messagebody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtp.Send(mail);
                smtp.Dispose();
            }
            catch (System.Net.Mail.SmtpException myEx)
            {
                return "Error: " + myEx.Message.ToString() + " " + myEx.StackTrace.ToString();
                throw myEx.InnerException;
            }


            var MesaageReply = "<html>" +
                                    "<body style='font-family: lato, Helvetica, sans-serif;font-size: 16px;width:600px;'>" +
                                        "<div style='padding: 15px 30px;background: #1d5fa5;color: #fff;'>" +
                                             "<h4 style='padding: 0;margin: 0;'>Project Inquiry Details</h4>" +
                                          "</div>" +
                                           "<div style='padding: 15px 30px;background: #f8f8f8;'>" +
                                                "<p>Dear " + projectDetail.FirstName + ",<br/><br/>" +
                                                    "Thank you for the interest in <strong>" + projectDetail.Service + "</strong> <br/><br/> Please follow the link below to apply for relevant position.<br/> <a href='https://www.avidclan.com/Avidclan/Services'>Apply Now</a><br/></br>We always love to hear from you. Our inbox can't wait to get your messages, so talk to us anytime you like! <br/><br/>Regards,<br/><br/>" +
                                                           "Team,<a href='https://www.avidclan.com'> Avidclan Technologies </a><br/><a href='mailto:info@avidclan.com'> info@avidclan.com </a><br/> +91 9624679717 <br/><br/> This mail is sent via portfolio form on Avidclan Technologies Site<br/><a href='https://www.avidclan.com'> https://www.avidclan.com</a>" +
                                                  "</p></div>" +
                                      "</body></html>";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                MailMessage replymail = new MailMessage();
                replymail.To.Add(new MailAddress(projectDetail.Email));
                replymail.From = new MailAddress(senderEmail, "Avidclan Technologies");
                replymail.Subject = "Project Inquiry From Avidclan Technologies";
                replymail.Body = MesaageReply;
                replymail.IsBodyHtml = true;
                SmtpClient smtpreplymail = new SmtpClient(host, 25);
                smtpreplymail.EnableSsl = true;
                smtpreplymail.UseDefaultCredentials = false;
                smtpreplymail.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtpreplymail.Send(replymail);


            }
            catch (System.Net.Mail.SmtpException myEx)
            {
                return "Error: " + myEx.Message.ToString() + " " + myEx.StackTrace.ToString();
                throw myEx.InnerException;
            }


            return "Sent";
        }


        [Route("api/Mail/SendCandidateDetails")]
        [HttpPost]
        public async Task<string> SendCandidateDetails()
        {
            await ReadConfiguration();
            var Name = HttpContext.Current.Request["Name"];
            var Email = HttpContext.Current.Request["Email"];
            var ContactNumber = HttpContext.Current.Request["ContactNumber"];
            var Message = HttpContext.Current.Request["Message"];
            var Position = HttpContext.Current.Request["Position"];
            var Resume = HttpContext.Current.Request.Files["Resume"];

            var messagebody = "<html>" +
                                    "<body style='font-family: lato, Helvetica, sans-serif;font-size: 16px;width:600px;'>" +
                                    "<div style='padding: 15px 30px;background: #1d5fa5;color: #fff;'>" +
                                        "<h4 style='padding: 0;margin: 0;'>Career Inquiry Details</h4>" +
                                     "</div>" +
                                      "<div style='padding: 15px 30px;background: #f8f8f8;'>" +
                                            "<p>Hello,<br/>" + Message + "<br/><br/>" +
                                            "<strong>Job Position:</strong>" + Position + "<br/><br/>" +
                                            "Regards,<br/><br/><strong>" + Name + "</strong><br/>" + Email + "<br/>" +
                                           ContactNumber + "<br/><br/><br/> This mail is sent via career form on Avidclan Technologies Web Site <br/><a href='https://www.avidclan.com'>https://www.avidclan.com</a>" +
                                        "</p></div>" +
                                "</body></html>";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(receiverEmail);
                mail.From = new MailAddress(senderEmail);
                mail.Subject = "Career Inquiry From Avidclan Technologies";
                mail.Body = messagebody;
                mail.IsBodyHtml = true;
                mail.Attachments.Add(new System.Net.Mail.Attachment(Resume.InputStream, Resume.FileName));
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtp.Send(mail);
            }
            catch (System.Net.Mail.SmtpException myEx)
            {
                return "Error: " + myEx.Message.ToString() + " " + myEx.StackTrace.ToString();
                throw myEx.InnerException;
            }
            var MessageReply = "<html>" +
                                   "<body style='font-family: lato, Helvetica, sans-serif;font-size: 16px;width:600px;'>" +
                                        "<div style='padding: 15px 30px;background: #1d5fa5;color: #fff;'>" +
                                            "<h4 style='padding: 0;margin: 0;'>Career Inquiry Details</h4>" +
                                         "</div>" +
                                         "<div style='padding: 15px 30px;background: #f8f8f8;'>" +
                                             "<p>Dear " + Name + ",<br/><br/> Thank you for the interest in <strong>" + Position + "</strong> " +
                                                "Position <br/><br/> Please follow the link below to apply for relevant position.<br/>" +
                                                "<a href='https://www.avidclan.com/Avidclan/ContactUs'>Apply Now</a><br/></br>" +
                                                "We always love to hear from you. Our inbox can't wait to get your messages, so talk to us anytime you like! <br/><br/>Regards,<br/><br/>" +
                                                 "Team,<a href='https://www.avidclan.com/'> Avidclan Technologies </a><br/><a href='mailto:info@avidclan.com'> info@avidclan.com </a><br/> +91 9624679717 <br/><br/> This mail is sent via portfolio form on Avidclan Technologies Site<br/><a href='https://www.avidclan.com'> https://www.avidclan.com</a>" +
                                             "</p></div>" +
                                "</body></html>";
            try
            {
                MailMessage replymail = new MailMessage();
                replymail.To.Add(Email);
                replymail.From = new MailAddress(senderEmail);
                replymail.Subject = "Career Inquiry From Avidclan Technologies";
                replymail.Body = MessageReply;
                replymail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                smtp.Send(replymail);
            }
            catch (System.Net.Mail.SmtpException myEx)
            {
                return "Error: " + myEx.Message.ToString() + " " + myEx.StackTrace.ToString();
                throw myEx.InnerException;
            }


            return "Sent";

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
    }
    public class EMailConfiguration
    {
        public string FromMail { get; set; }
        public string ToMail { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public int Id { get; set; }
    }
}
