using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using Ionic.Zip;
using iTextSharp.text.pdf;
using iTextSharp.text;
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
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Services.Description;
using iTextSharp.tool.xml.html;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Xml.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System.Threading;
using System.Web.Helpers;
using MimeKit;
using System.Web.Http.Results;

namespace Avidclan_BlogsVacancy.Controllers
{
    public class SalaryWebApiController : ApiController
    {
        string senderEmail = "";
        string senderEmailPassword = "";
        string host = "";
        int port = 0;
        string receiverEmail = "";

        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public SalaryWebApiController()
        {
            con = new SqlConnection(connectionString);
        }

        [System.Web.Http.Route("api/SalaryWebApiController/DownloadSalarySlip")]
        [System.Web.Http.HttpPost]
        public async Task<string> DownloadSalarySlip(string EmployesData)
        {
            SalarySlipViewModel[] arr = JsonConvert.DeserializeObject<SalarySlipViewModel[]>(EmployesData);
            ZipFile zipFile = new ZipFile();
            string fileName = string.Empty;
            var Slipdate = "";
            string fileType = string.Empty;
            MemoryStream memoryStream = null;

            for (int i = 0; i < arr.Length; i++)
            {
                Slipdate = arr[i].Date.ToString("MMMM yyyy");
                string monthName = Slipdate.Split(' ')[0];
                string year = Slipdate.Split(' ')[1];

                fileType = arr[i].Type;

                //CONVERTING MONTH NAME TO MONTH NUMBER
                int month = DateTime.ParseExact(monthName, "MMMM", CultureInfo.CurrentCulture).Month;

                //GETTING TOTAL DAYS IN MONTH
                int totalDaysInMonth = DateTime.DaysInMonth(Convert.ToInt16(year), month);

                //COUNT WEEKENDS OF THE MONTH
                var totalWeekendsinMonths = CountWeekends(Convert.ToInt16(year), month);

                //COUNT PAID DAYS
                //var HolidayList = HolidaysDate(month, Convert.ToInt16(year));
                var LeaveDays = CountWeekendsInholiday(month, Convert.ToInt16(year));
                LeaveDays = totalDaysInMonth - totalWeekendsinMonths - LeaveDays;

                /*Basic Salary Count*/
                int SalaryToInt = int.Parse(arr[i].Salary.Replace(",", ""));
                var basicsalary = SalaryToInt * 0.5;


                /*HRA Salary Count*/
                var hrasalary = (SalaryToInt * 10) / 100;


                /*Special Allowance*/
                var specialallownace = SalaryToInt - (basicsalary + hrasalary + 240 + 300);


                /*Total Earnings*/
                var totalEarning = basicsalary + hrasalary + 240 + 300 + specialallownace;

                /*CONVERTING SALARY NUMBERS TO WORDS*/
                var SalaryInWords = NumberToWords(SalaryToInt);

                Document doc = new Document(PageSize.A4, 7f, 5f, 20f, 0f);

                try
                {
                    memoryStream = new MemoryStream();
                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    var italicFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 12);

                    PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                    doc.Open();
                    PdfPTable table = new PdfPTable(4);

                    PdfPCell companyname = new PdfPCell(new Phrase("AVIDCLAN TECHNOLOGIES", boldFont));

                    companyname.Colspan = 4;
                    companyname.HorizontalAlignment = 1;
                    table.AddCell(companyname);

                    PdfPCell companyaddress = new PdfPCell(new Phrase("1101 Shivalik Shilp Iskcon Cross Rd, Sarkhej - Gandhinagar Hwy, Ahmedabad, Gujarat 380015", boldFont));
                    companyaddress.Colspan = 4;
                    companyaddress.HorizontalAlignment = 1;
                    table.AddCell(companyaddress);

                    PdfPCell slipdate = new PdfPCell(new Phrase("Pay Slip for the Month " + Slipdate, boldFont));
                    slipdate.BackgroundColor = BaseColor.LIGHT_GRAY;
                    slipdate.Colspan = 4;
                    slipdate.HorizontalAlignment = 1;
                    table.AddCell(slipdate);

                    PdfPCell blankspace = new PdfPCell(new Phrase(" "));
                    blankspace.Colspan = 4;
                    blankspace.HorizontalAlignment = 1;
                    table.AddCell(blankspace);

                    PdfPCell employeeName = new PdfPCell(new Phrase("Name", boldFont));
                    //employeeName.DisableBorderSide(3);
                    fileName = arr[i].FullName;
                    employeeName.PaddingLeft = 10f;
                    table.AddCell(employeeName);

                    PdfPCell Name = new PdfPCell(new Phrase(":" + arr[i].FullName));
                    employeeName.DisableBorderSide(4);

                    table.AddCell(Name);

                    PdfPCell calendarDays = new PdfPCell(new Phrase("Calendar Days", boldFont));
                    table.AddCell(calendarDays);
                    table.AddCell(": " + totalDaysInMonth);

                    PdfPCell empId = new PdfPCell(new Phrase("Emp ID", boldFont));
                    empId.PaddingLeft = 10f;
                    table.AddCell(empId);
                    table.AddCell(": " + arr[i].EmpId);

                    PdfPCell weeklyOff = new PdfPCell(new Phrase("Weekly Off", boldFont));
                    table.AddCell(weeklyOff);
                    table.AddCell(": " + totalWeekendsinMonths);

                    PdfPCell designation = new PdfPCell(new Phrase("Designation", boldFont));
                    designation.PaddingLeft = 10f;
                    table.AddCell(designation);
                    table.AddCell(": " + arr[i].Designation);

                    PdfPCell leavewithoutPay = new PdfPCell(new Phrase("Leave Without Pay", boldFont));
                    table.AddCell(leavewithoutPay);
                    table.AddCell(": " + arr[i].LeaveWithoutpay);

                    PdfPCell dateOfJoining = new PdfPCell(new Phrase("Date of Joining", boldFont));
                    //  var joiningdate = arr[i].DateOfJoining.ToString("dd-MMMM-yyyy");
                    dateOfJoining.PaddingLeft = 10f;
                    table.AddCell(dateOfJoining);
                    table.AddCell(": " + arr[i].DateOfJoining);

                    PdfPCell paidDays = new PdfPCell(new Phrase("Paid Days", boldFont));
                    table.AddCell(paidDays);
                    table.AddCell(":" + LeaveDays);

                    PdfPCell ctcPermonth = new PdfPCell(new Phrase("CTC (Per Month)", boldFont));
                    ctcPermonth.PaddingLeft = 10f;
                    table.AddCell(ctcPermonth);
                    PdfPCell salary = new PdfPCell(new Phrase(":" + arr[i].Salary));
                    salary.Colspan = 3;
                    table.AddCell(salary);

                    PdfPCell blankspace1 = new PdfPCell(new Phrase(" "));
                    blankspace1.Colspan = 4;
                    blankspace1.HorizontalAlignment = 1;
                    table.AddCell(blankspace1);

                    //ADDING Earning NESTED TABLE
                    PdfPTable earningNestedTable = new PdfPTable(2);
                    PdfPCell earning = new PdfPCell(new Phrase("Earnings", boldFont));
                    earning.PaddingLeft = 30f;
                    earning.BackgroundColor = BaseColor.LIGHT_GRAY;
                    earningNestedTable.AddCell(earning);

                    PdfPCell amount = new PdfPCell(new Phrase("Amount", boldFont));
                    amount.PaddingLeft = 30f;
                    amount.BackgroundColor = BaseColor.LIGHT_GRAY;
                    earningNestedTable.AddCell(amount);

                    earningNestedTable.AddCell("Basic");
                    earningNestedTable.AddCell(basicsalary.ToString());

                    earningNestedTable.AddCell("HRA");
                    earningNestedTable.AddCell(hrasalary.ToString());

                    earningNestedTable.AddCell("CLA");
                    earningNestedTable.AddCell("240");

                    earningNestedTable.AddCell("Medical Allowance");
                    earningNestedTable.AddCell("300");

                    PdfPCell specialallowance = new PdfPCell(new Phrase("Special Allowance"));
                    specialallowance.PaddingBottom = 30;
                    earningNestedTable.AddCell(specialallowance);
                    PdfPCell specialallowanceamount = new PdfPCell(new Phrase(specialallownace.ToString()));
                    specialallowanceamount.PaddingBottom = 30;
                    earningNestedTable.AddCell(specialallowanceamount);

                    PdfPCell totalEarnings = new PdfPCell(new Phrase("Total Earnings", boldFont));
                    totalEarnings.PaddingLeft = 10f;
                    earningNestedTable.AddCell(totalEarnings);
                    earningNestedTable.AddCell(totalEarning.ToString());
                    //FINSH ADDING Earning NESTED TABLE

                    //ADDING TAX NESTED TABLE
                    PdfPTable taxNestedTable = new PdfPTable(2);
                    PdfPCell deductions = new PdfPCell(new Phrase("Deductions", boldFont));
                    deductions.PaddingLeft = 30f;
                    deductions.BackgroundColor = BaseColor.LIGHT_GRAY;
                    taxNestedTable.AddCell(deductions);

                    PdfPCell taxAmount = new PdfPCell(new Phrase("Amount", boldFont));
                    taxAmount.PaddingLeft = 30f;
                    taxAmount.BackgroundColor = BaseColor.LIGHT_GRAY;
                    taxNestedTable.AddCell(taxAmount);

                    PdfPCell proffesionaltax = new PdfPCell(new Phrase("Proffesional Tax"));
                    proffesionaltax.PaddingBottom = 94;
                    taxNestedTable.AddCell(proffesionaltax);
                    PdfPCell proffesionaltaxamount = new PdfPCell(new Phrase("200.00"));
                    proffesionaltaxamount.PaddingBottom = 94;
                    taxNestedTable.AddCell(proffesionaltaxamount);

                    PdfPCell totalDeductions = new PdfPCell(new Phrase("Total Deductions", boldFont));
                    totalDeductions.PaddingLeft = 10f;
                    taxNestedTable.AddCell(totalDeductions);
                    taxNestedTable.AddCell("200.00");

                    //FINISH ADDING TAX NESTED TABLE

                    PdfPCell earningtable = new PdfPCell(earningNestedTable);
                    earningtable.Colspan = 2;
                    table.AddCell(earningtable);

                    PdfPCell taxtable = new PdfPCell(taxNestedTable);
                    taxtable.Colspan = 2;
                    table.AddCell(taxtable);

                    PdfPCell netsalary = new PdfPCell(new Phrase("Net Salary", boldFont));
                    netsalary.PaddingLeft = 30f;
                    table.AddCell(netsalary);
                    PdfPCell totalsalary = new PdfPCell(new Phrase(":" + arr[i].Salary + "/-"));
                    totalsalary.PaddingLeft = 10f;
                    totalsalary.Colspan = 3;
                    table.AddCell(totalsalary);

                    PdfPCell salaryInwords = new PdfPCell(new Phrase("In Words", boldFont));
                    salaryInwords.PaddingLeft = 30f;
                    table.AddCell(salaryInwords);
                    PdfPCell totalsalaryInwords = new PdfPCell(new Phrase(":" + SalaryInWords + "Rupees only"));
                    totalsalaryInwords.PaddingLeft = 10f;
                    totalsalaryInwords.Colspan = 3;
                    table.AddCell(totalsalaryInwords);

                    PdfPCell note = new PdfPCell(new Phrase("* This is computer generated payslip and does not require any signature/company stamp.", italicFont));
                    note.PaddingLeft = 10f;
                    note.Colspan = 4;
                    table.AddCell(note);


                    doc.Add(table);
                    doc.Close();
                   
                    if (arr[i].Type == "SendMail")
                    {
                        await ReadConfiguration();
                        byte[] bytes = memoryStream.ToArray();
                        MailMessage mail = new MailMessage();
                        mail.To.Add("pooja.avidclan@gmail.com");
                        mail.From = new MailAddress(senderEmail);
                        mail.Subject = "Below is your salary slip of month " + Slipdate;
                        mail.Body = "Salary Slip";
                        mail.IsBodyHtml = true;
                        mail.Attachments.Add(new Attachment(new MemoryStream(bytes), fileName + ".pdf"));
                        SmtpClient smtp = new SmtpClient(host, port);
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                        smtp.Send(mail);

                    }
                }
                catch (Exception ex)
                {
                    var result = ex.Message + ex.StackTrace;
                    return result;
                }
            }

            return "Mail Send Successfully";

        }

        public int CountWeekends(int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var weekendDays = 0;
            while (firstDayOfMonth <= lastDayOfMonth)
            {
                var weekday = firstDayOfMonth.DayOfWeek.ToString();
                if (weekday == "Saturday" || weekday == "Sunday")
                {
                    weekendDays++;
                }
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }

            //throw new NotImplementedException();
            return weekendDays;
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        public int CountWeekendsInholiday(int Month, int Year)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Month", Month, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Year", Year, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var HolidayList = con.Query<SalarySlipViewModel>("sp_Holidays", parameters, commandType: CommandType.StoredProcedure).ToList();
            var holidaysweekendDays = 0;
            if (HolidayList.Count > 0)
            {
                foreach (var date in HolidayList)
                {
                    var dates = date.HolidaysDate;
                    var weekday = dates.DayOfWeek.ToString();
                    if (weekday == "Saturday" || weekday == "Sunday")
                    {
                        holidaysweekendDays++;
                    }
                }
            }

            return holidaysweekendDays;
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

}

