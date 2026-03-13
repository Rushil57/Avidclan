using Avidclan_Website.Models;
using Dapper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
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
                if (obj == null)
                    return "Invalid request.";

                if (string.IsNullOrEmpty(obj.FirstName) ||
                    string.IsNullOrEmpty(obj.Email) ||
                    string.IsNullOrEmpty(obj.Phoneumber) ||
                    string.IsNullOrEmpty(obj.RecaptchaToken))
                {
                    return "All fields are required.";
                }

                var secretKey = ConfigurationManager.AppSettings["reCaptchaPrivateKey"];

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", obj.RecaptchaToken }
            };

                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    var captchaResult = JsonConvert.DeserializeObject<RecaptchaResponse>(responseString);

                    if (!captchaResult.success ||
                        captchaResult.score < 0.5 ||
                        captchaResult.action != "contact_form")
                    {
                        ErrorLog("ContactForm", "reCAPTCHA Failed", responseString);
                        return "reCAPTCHA validation failed.";
                    }
                }

                // Mail to info@avidclan.com
                await ReadConfiguration("other");

                var messagebody = "<html><body>" +
                                  "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                                  "<tr><td><strong>FirstName:</strong></td><td>" + obj.FirstName + "</td></tr>" +
                                  "<tr><td><strong>LastName:</strong></td><td>" + obj.LastName + "</td></tr>" +
                                  "<tr><td><strong>Email:</strong></td><td>" + obj.Email + "</td></tr>" +
                                  "<tr><td><strong>Mobile:</strong></td><td>+" + obj.CountryCode + " " + obj.Phoneumber + "</td></tr>" +
                                  "<tr><td><strong>Message:</strong></td><td>" + obj.Message + "</td></tr>" +
                                  "</table></body></html>";

                await sendEmail(senderEmail, receiverEmail,
                    obj.FirstName + " " + obj.LastName,
                    "Contact Inquiry From Avidclan Technologies",
                    messagebody);

                // Mail to solution@avidclan.com
                await ReadConfiguration("solution");

                await sendEmail(senderEmail, receiverEmail,
                    obj.FirstName + " " + obj.LastName,
                    "Contact Inquiry From Avidclan Technologies",
                    messagebody);

                ErrorLog("ContactForm", "Execution Success", "Success");

                return "Sent";
            }
            catch (Exception ex)
            {
                ErrorLog("ContactForm", ex.Message, ex.StackTrace);
                return "Error: " + ex.Message;
            }
        }
        [Route("api/Mail/SendEmailMarketingPage")]
        [HttpPost]
        public async Task<string> SendEmailMarketingPage(UserDetail obj)
        {
            try
            {
                await ReadConfiguration("other");
                var messagebody = "<html><body>" +
                                    "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                                    "<tr style='background: #eee;'><td colspan='2'><strong>Contact Inquiry Details</strong></td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Gclid:</strong> </td><td>" + obj.Gclid + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Name:</strong> </td><td>" + obj.FirstName + " </td></tr>" +
                                    //"<tr style='background: #fff;'><td><strong>LastName:</strong> </td><td>" + obj.LastName + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Email:</strong> </td><td>" + obj.Email + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Mobile/Phone:</strong> </td><td>" + "+" + obj.Phoneumber + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Requirements:</strong> </td><td>" + obj.Message + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Service:</strong> </td><td>" + obj.Service + " </td></tr>" +
                                    "<tr style='background: #fff;'><td><strong>Budget:</strong> </td><td>" + obj.Budget + " </td></tr>" +
                                   "</table>" +
                                   "</body></html>";

                //MailMessage mail = new MailMessage();
                //mail.To.Add(receiverEmail);
                //mail.From = new MailAddress(senderEmail);
                //mail.Subject = "Contact Inquiry From Avidclan Technologies";
                //mail.Body = messagebody;
                //mail.IsBodyHtml = true;
                //SmtpClient smtp = new SmtpClient(host, port);
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                //smtp.Send(mail);

                await sendEmail(senderEmail, receiverEmail, (obj.FirstName + " " + obj.LastName), "Contact Inquiry From Avidclan Technologies", messagebody);

                //mail send on solutions@avidclan.com
                await ReadConfiguration("solution");
                await sendEmail(senderEmail, receiverEmail, (obj.FirstName + " " + obj.LastName), "Contact Inquiry From Avidclan Technologies", messagebody);

                ErrorLog("Mail", "Execution Success", "Success");
            }
            catch (Exception ex)
            {
                ErrorLog("Mail", ex.Message.ToString() + ex.InnerException, ex.StackTrace.ToString());
                return "Error: " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
                throw ex.InnerException;
            }
            return "Sent";
        }
        [Route("api/Mail/SendProjectDetails")]
        [HttpPost]
        public async Task<string> SendProjectDetails(ProjectDetail projectDetail)
        {
            try
            {
                var secretKey = ConfigurationManager.AppSettings["reCaptchaPrivateKey"]; 

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(
                        $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={projectDetail.RecaptchaToken}",
                        null);

                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

                    if (!(jsonData.success == true && jsonData.score >= 0.5))
                    {
                        ErrorLog("ProjectForm", "reCAPTCHA Failed", jsonData);

                        return "reCAPTCHA validation failed.";
                    }
                }


                await ReadConfiguration("other");

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
                    "Regards,<br/><br/><strong>" + projectDetail.FirstName + " " + projectDetail.LastName + "</strong><br/>" +
                    projectDetail.Email + "<br/>+" + projectDetail.CountryCode + " " + projectDetail.Phone +
                    "<br/><br/><br/>This mail is sent via project form on Avidclan Technologies Web Site <br/>" +
                    "<a href='https://www.avidclan.com'>https://www.avidclan.com</a>" +
                    "</p></div></body></html>";

                await sendEmail(senderEmail, receiverEmail,
                    (projectDetail.FirstName + " " + projectDetail.LastName),
                    "Project Inquiry From Avidclan Technologies",
                    messagebody);

                await ReadConfiguration("solution");

                await sendEmail(senderEmail, receiverEmail,
                    (projectDetail.FirstName + " " + projectDetail.LastName),
                    "Project Inquiry From Avidclan Technologies",
                    messagebody);
                ErrorLog("ProjectForm", "Execution Success", "Success");

                return "Sent";
            }
            catch (Exception ex)
            {
                ErrorLog("ProjectForm", ex.Message, ex.StackTrace);
                return "Error: " + ex.Message;
            }
        }

        [Route("api/Mail/SendCandidateDetails")]
        [HttpPost]
        public async Task<string> SendCandidateDetails()
        {
            await ReadConfiguration("career");
            var Name = HttpContext.Current.Request["Name"];
            var Email = HttpContext.Current.Request["Email"];
            var ContactNumber = HttpContext.Current.Request["ContactNumber"];
            var Message = HttpContext.Current.Request["Message"];
            var Position = HttpContext.Current.Request["Position"];
            var RecaptchaToken = HttpContext.Current.Request["RecaptchaToken"];

            var Resume = HttpContext.Current.Request.Files["Resume"];

            if (string.IsNullOrEmpty(RecaptchaToken))
                return "Captcha validation failed.";

            var secretKey = ConfigurationManager.AppSettings["reCaptchaPrivateKey"];
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={RecaptchaToken}",
                    null);

                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

                if (result.success != true || result.score < 0.5)
                {
                    return "Captcha verification failed. You might be a bot.";
                }
            }
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
                //MailMessage mail = new MailMessage();
                //mail.To.Add(receiverEmail);
                //mail.From = new MailAddress(senderEmail);
                //mail.Subject = "Career Inquiry From Avidclan Technologies";
                //mail.Body = messagebody;
                //mail.IsBodyHtml = true;
                //mail.Attachments.Add(new System.Net.Mail.Attachment(Resume.InputStream, Resume.FileName));
                //SmtpClient smtp = new SmtpClient(host, port);
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                //smtp.Send(mail);

                string base64Attachment = ConvertAttachmentToBase64(Resume.InputStream, Resume.FileName);
                await sendEmail(senderEmail, receiverEmail, Name, "Career Inquiry From Avidclan Technologies", messagebody, base64Attachment, Resume.ContentType);
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
                                                "<a href='https://www.avidclan.com/contact-us/'>Apply Now</a><br/></br>" +
                                                "We always love to hear from you. Our inbox can't wait to get your messages, so talk to us anytime you like! <br/><br/>Regards,<br/><br/>" +
                                                 "Team,<a href='https://www.avidclan.com/'> Avidclan Technologies </a><br/><a href='mailto:info@avidclan.com'> info@avidclan.com </a><br/> +91 9624679717 <br/><br/> This mail is sent via portfolio form on Avidclan Technologies Site<br/><a href='https://www.avidclan.com'> https://www.avidclan.com</a>" +
                                             "</p></div>" +
                                "</body></html>";
            try
            {
                //MailMessage replymail = new MailMessage();
                //replymail.To.Add(Email);
                //replymail.From = new MailAddress(senderEmail);
                //replymail.Subject = "Career Inquiry From Avidclan Technologies";
                //replymail.Body = MessageReply;
                //replymail.IsBodyHtml = true;
                //SmtpClient smtp = new SmtpClient(host, port);
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                //smtp.Send(replymail);

                //await sendEmail(senderEmail, Email, Name, "Career Inquiry From Avidclan Technologies", MessageReply);
            }
            catch (System.Net.Mail.SmtpException myEx)
            {
                return "Error: " + myEx.Message.ToString() + " " + myEx.StackTrace.ToString();
                throw myEx.InnerException;
            }


            return "Sent";

        }

        [Route("api/Mail/SendHiringDotNetDetails")]
        [HttpPost]
        public async Task<string> SendHiringDotNetDetails(HiringCandidateDetails obj)
        {
            try
            {
                if (obj == null)
                    return "Invalid request.";

                if (string.IsNullOrEmpty(obj.Name) ||
                    string.IsNullOrEmpty(obj.Email) ||
                    string.IsNullOrEmpty(obj.PhoneNumber) ||
                    string.IsNullOrEmpty(obj.RecaptchaToken))
                {
                    return "All fields are required.";
                }

                var secretKey = ConfigurationManager.AppSettings["reCaptchaPrivateKey"];

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", obj.RecaptchaToken }
            };

                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    var captchaResult = JsonConvert.DeserializeObject<RecaptchaResponse>(responseString);

                    if (!captchaResult.success ||
                        captchaResult.score < 0.5 ||
                        captchaResult.action != "hire_form")
                    {
                        ErrorLog("HiringForm", "reCAPTCHA Failed", responseString);
                        return "reCAPTCHA validation failed.";
                    }
                }

                var Position = "DotNet Developer";

                var messagebody = "<html><body>" +
                                  "<h4>Inquiry for Hiring .Net Developers</h4>" +
                                  "<p>" + obj.Message + "<br/><br/>" +
                                  "<strong>Position:</strong> " + Position + "<br/><br/>" +
                                  "<strong>Name:</strong> " + obj.Name + "<br/>" +
                                  "<strong>Email:</strong> " + obj.Email + "<br/>" +
                                  "<strong>Phone:</strong> +" + obj.CountryCode + " " + obj.PhoneNumber +
                                  "</p></body></html>";

                // Mail to info@avidclan.com
                await ReadConfiguration("other");

                await sendEmail(senderEmail, receiverEmail,
                    obj.Name,
                    "Inquiry for Hiring .Net Developers",
                    messagebody);

                // Mail to solution@avidclan.com
                await ReadConfiguration("solution");

                await sendEmail(senderEmail, receiverEmail,
                    obj.Name,
                    "Inquiry for Hiring .Net Developers",
                    messagebody);

                ErrorLog("HiringForm", "Execution Success", "Success");

                return "Sent";
            }
            catch (Exception ex)
            {
                ErrorLog("HiringForm", ex.Message, ex.StackTrace);
                return "Error: " + ex.Message;
            }
        }
        [Route("api/Mail/SendLocationPageDetails")]
        [HttpPost]
        public async Task<string> SendLocationPageDetails(UserDetail obj)
        {
            try
            {
                if (obj == null || string.IsNullOrEmpty(obj.RecaptchaToken))
                    return "Invalid request.";

                var secretKey = ConfigurationManager.AppSettings["reCaptchaPrivateKey"];

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", obj.RecaptchaToken }
            };

                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    var captchaResult = JsonConvert.DeserializeObject<RecaptchaResponse>(responseString);

                    if (!captchaResult.success || captchaResult.score < 0.5)
                    {
                        ErrorLog("LocationForm", "reCAPTCHA Failed", responseString);
                        return "reCAPTCHA validation failed.";
                    }
                }

                await ReadConfiguration("other");

                var messagebody = "<html><body>" +
                                  "<table rules='all' style='border:1px solid #666;' cellpadding='10'>" +
                                  "<tr><td><strong>FirstName:</strong></td><td>" + obj.FirstName + "</td></tr>" +
                                  "<tr><td><strong>LastName:</strong></td><td>" + obj.LastName + "</td></tr>" +
                                  "<tr><td><strong>Email:</strong></td><td>" + obj.Email + "</td></tr>" +
                                  "<tr><td><strong>Phone:</strong></td><td>+" + obj.CountryCode + " " + obj.Phoneumber + "</td></tr>" +
                                  "<tr><td><strong>Message:</strong></td><td>" + obj.Message + "</td></tr>" +
                                  "</table></body></html>";

                await sendEmail(senderEmail, receiverEmail,
                    obj.FirstName + " " + obj.LastName,
                    "Service Inquiry From Avidclan Technologies",
                    messagebody);

                await ReadConfiguration("solution");

                await sendEmail(senderEmail, receiverEmail,
                    obj.FirstName + " " + obj.LastName,
                    "Service Inquiry From Avidclan Technologies",
                    messagebody);

                ErrorLog("LocationForm", "Execution Success", "Success");

                return "Sent";
            }
            catch (Exception ex)
            {
                ErrorLog("LocationForm", ex.Message, ex.StackTrace);
                return "Error: " + ex.Message;
            }
        }
        public async Task<bool> ReadConfiguration(string type)
        {
            var result = false;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Type", type, DbType.String, ParameterDirection.Input);
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

        public void ErrorLog(string ControllerName, string ErrorMessage, string StackTrace)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ControllerName", ControllerName, DbType.String, ParameterDirection.Input);
            parameters.Add("@ErrorMessage", ErrorMessage, DbType.String, ParameterDirection.Input);
            parameters.Add("@StackTrace", StackTrace, DbType.String, ParameterDirection.Input);
            parameters.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
            var SaveError = con.ExecuteScalar("sp_Errorlog", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task sendEmail(string fromEmail, string toEmail, string reciverName, string subject, string message, string base64 = "", string fileType = "")
        {
            string apiKey = "a101ac38119207e6774e78a74701c990";
            string apiSecret = "fc46b6850d50f957b087e2ba1bf2c0ee";
            string apiUrl = "https://api.mailjet.com/v3.1/send";
            using (HttpClient client = new HttpClient())
            {
                string base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);
                //string emailPayload = "{\"Messages\":[{\"From\":{\"Email\":\""+fromEmail+"\",\"Name\":\"avidclan\"},\"To\":[{\"Email\":\""+toEmail+"\",\"Name\":\"avidclan\"}],\"Subject\":\""+ subject + "\",\"HTMLPart\":\"" + message+"\"}]}";
                string emailPayload = "";
                if (!base64.IsNullOrWhiteSpace())
                {
                    if (fileType == "application/msword" || fileType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                    {
                        emailPayload = $"{{\"Messages\":[{{\"From\":{{\"Email\":\"{fromEmail}\",\"Name\":\"Avidclan Technologies\"}},\"To\":[{{\"Email\":\"{toEmail}\",\"Name\":\"{reciverName}\"}}],\"Subject\":\"{subject}\",\"HTMLPart\":\"{message}\",\"Attachments\":[{{\"ContentType\":\"application/msword\",\"Filename\":\"attachment.doc\",\"Base64Content\":\"{base64}\"}}] }}]}}";
                    }
                    else if (fileType == "application/pdf")
                    {
                        emailPayload = $"{{\"Messages\":[{{\"From\":{{\"Email\":\"{fromEmail}\",\"Name\":\"Avidclan Technologies\"}},\"To\":[{{\"Email\":\"{toEmail}\",\"Name\":\"{reciverName}\"}}],\"Subject\":\"{subject}\",\"HTMLPart\":\"{message}\",\"Attachments\":[{{\"ContentType\":\"application/pdf\",\"Filename\":\"attachment.pdf\",\"Base64Content\":\"{base64}\"}}] }}]}}";
                    }
                }
                else
                {
                    emailPayload = $"{{\"Messages\":[{{\"From\":{{\"Email\":\"{fromEmail}\",\"Name\":\"Avidclan Technologies\"}},\"To\":[{{\"Email\":\"{toEmail}\",\"Name\":\"{reciverName}\"}}],\"Subject\":\"{subject}\",\"HTMLPart\":\"{message}\"}}]}}";
                }

                StringContent content = new StringContent(emailPayload, Encoding.UTF8, "application/json");
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

        static string ConvertAttachmentToBase64(Stream attachmentStream, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                attachmentStream.CopyTo(memoryStream);
                byte[] attachmentBytes = memoryStream.ToArray();
                string base64Attachment = Convert.ToBase64String(attachmentBytes);
                return base64Attachment;
            }
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