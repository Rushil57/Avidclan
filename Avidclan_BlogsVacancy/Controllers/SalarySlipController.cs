using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avidclan_BlogsVacancy.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Ionic.Zip;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Globalization;
using iTextSharp.tool.xml.html;
using Dapper;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using System.Xml.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Services.Description;
using iTextSharp.text.pdf.qrcode;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

namespace Avidclan_BlogsVacancy.Controllers
{
    public class SalarySlipController : Controller
    {
        string senderEmail = "";
        string senderEmailPassword = "";
        string host = "";
        int port = 0;
        string receiverEmail = "";

        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public SalarySlipController()
        {
            con = new SqlConnection(connectionString);
        }


        // GET: SalarySlip
        public ActionResult Index()
        {
            return View();
        }

        public async void DownloadSalarySlip(string EmployesData)
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
                LeaveDays = totalDaysInMonth - totalWeekendsinMonths - LeaveDays- arr[i].LeaveWithoutpay;

                /*Basic Salary Count*/
                int SalaryToInt = int.Parse(arr[i].Salary.Replace(",", ""));
                var basicsalary = SalaryToInt * 0.5;


                /*HRA Salary Count*/
                var hrasalary = (SalaryToInt * 20) / 100;


                /*Special Allowance*/
                var specialallownace = SalaryToInt - (basicsalary + hrasalary + 240 + 300);

                /*Non Working days*/
                var NonWorkingDays = 0;
                var DateofJoining = arr[i].DateOfJoining;
                string joindate = DateofJoining.Split('-')[0];
                string Joinmonth = DateofJoining.Split('-')[1];
                string JoinYear = DateofJoining.Split('-')[2];
                if (JoinYear == year && Joinmonth == monthName)
                {
                    int joinmonthnumber = DateTime.ParseExact(Joinmonth, "MMMM", CultureInfo.CurrentCulture).Month;
                    NonWorkingDays = CountNonWorkingDays(joinmonthnumber, Convert.ToInt16(JoinYear), DateofJoining);
                }
                /*Non Working days*/

                /*Total Earnings*/
                var totalEarning = basicsalary + hrasalary + 240 + 300 + specialallownace;

                /*Unpaid Days*/
                var totalworkingdays = totalDaysInMonth - totalWeekendsinMonths ;
                var totalpaidDays = totalworkingdays - arr[i].LeaveWithoutpay - NonWorkingDays;
                var salaryperday = SalaryToInt / totalworkingdays;
                //salaryperday = (int)Decimal.Truncate(salaryperday);
                salaryperday = (int)Decimal.Round(salaryperday);
                var TotalsalaryCount = salaryperday * totalpaidDays;
                var deductionofleave = salaryperday * arr[i].LeaveWithoutpay;
               // deductionofleave = (int)Decimal.Truncate(deductionofleave);
                var deductionofnonworkingdays = salaryperday * NonWorkingDays;
                //deductionofnonworkingdays = (int)Decimal.Truncate(deductionofnonworkingdays);
                /*Unpaid Days*/

                /*Net Salary In Numbers*/
                var netSalaryInnumbers = TotalsalaryCount - 200;
                /*Net Salary In Numbers*/


                /*CONVERTING SALARY NUMBERS TO WORDS*/
                var SalaryInWords = NumberToWords(netSalaryInnumbers);

                var TotalamountDeductions = deductionofleave + 200 + deductionofnonworkingdays;

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

                    PdfPCell Name = new PdfPCell(new Phrase(" " + arr[i].FullName));
                    employeeName.DisableBorderSide(4);

                    table.AddCell(Name);

                    PdfPCell calendarDays = new PdfPCell(new Phrase("Calendar Days", boldFont));
                    calendarDays.PaddingLeft = 10f;
                    table.AddCell(calendarDays);
                    table.AddCell(" " + totalDaysInMonth);

                    PdfPCell empId = new PdfPCell(new Phrase("Emp ID", boldFont));
                    empId.PaddingLeft = 10f;
                    table.AddCell(empId);
                    table.AddCell(" " + arr[i].EmpId);

                    PdfPCell weeklyOff = new PdfPCell(new Phrase("Weekly Off", boldFont));
                    weeklyOff.PaddingLeft = 10f;
                    table.AddCell(weeklyOff);
                    table.AddCell(" " + totalWeekendsinMonths);

                    PdfPCell designation = new PdfPCell(new Phrase("Designation", boldFont));
                    designation.PaddingLeft = 10f;
                    table.AddCell(designation);
                    table.AddCell(" " + arr[i].Designation);

                    PdfPCell leavewithoutPay = new PdfPCell(new Phrase("Leave Without Pay", boldFont));
                    leavewithoutPay.PaddingLeft = 10f;
                    table.AddCell(leavewithoutPay);
                    table.AddCell(" " + arr[i].LeaveWithoutpay);

                    PdfPCell dateOfJoining = new PdfPCell(new Phrase("Date of Joining", boldFont));
                    //  var joiningdate = arr[i].DateOfJoining.ToString("dd-MMMM-yyyy");
                    dateOfJoining.PaddingLeft = 10f;
                    table.AddCell(dateOfJoining);
                    table.AddCell(" " + arr[i].DateOfJoining);

                    PdfPCell paidDays = new PdfPCell(new Phrase("Paid Days", boldFont));
                    paidDays.PaddingLeft = 10f;
                    table.AddCell(paidDays);
                    table.AddCell(" " + LeaveDays);

                    PdfPCell ctcPermonth = new PdfPCell(new Phrase("CTC (Per Month)", boldFont));
                    ctcPermonth.PaddingLeft = 10f;
                    table.AddCell(ctcPermonth);
                    PdfPCell salary = new PdfPCell(new Phrase(" " + arr[i].Salary));
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

                    //earningNestedTable.AddCell("Basic");
                    PdfPCell basicSalaryAmount = new PdfPCell(new Phrase("Basic"));
                    basicSalaryAmount.PaddingLeft = 10f;
                    earningNestedTable.AddCell(basicSalaryAmount);
                    earningNestedTable.AddCell(" " + basicsalary.ToString());

                   // earningNestedTable.AddCell("HRA");
                    PdfPCell hraAmount = new PdfPCell(new Phrase("HRA"));
                    hraAmount.PaddingLeft = 10f;
                    earningNestedTable.AddCell(hraAmount);
                    earningNestedTable.AddCell(" " + hrasalary.ToString());

                    //earningNestedTable.AddCell("CLA");
                    PdfPCell claAmount = new PdfPCell(new Phrase("CLA"));
                    claAmount.PaddingLeft = 10f;
                    earningNestedTable.AddCell(claAmount);
                    earningNestedTable.AddCell(" 240");

                    //earningNestedTable.AddCell("Medical Allowance");
                    PdfPCell medicalAmount = new PdfPCell(new Phrase("Medical Allowance"));
                    medicalAmount.PaddingLeft = 10f;
                    earningNestedTable.AddCell(medicalAmount);
                    earningNestedTable.AddCell(" 300");

                    PdfPCell specialallowance = new PdfPCell(new Phrase("Special Allowance"));
                    specialallowance.PaddingLeft = 10f;
                    specialallowance.PaddingBottom = 30;
                    earningNestedTable.AddCell(specialallowance);
                    PdfPCell specialallowanceamount = new PdfPCell(new Phrase(" " + specialallownace.ToString()));
                    specialallowanceamount.PaddingBottom = 30;
                    earningNestedTable.AddCell(specialallowanceamount);

                    PdfPCell totalEarnings = new PdfPCell(new Phrase("Total Earnings", boldFont));
                    totalEarnings.PaddingLeft = 10f;
                    earningNestedTable.AddCell(totalEarnings);
                    earningNestedTable.AddCell(" " + totalEarning.ToString());
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

                    PdfPCell proffesionaltax = new PdfPCell(new Phrase("Professional Tax"));
                    proffesionaltax.PaddingLeft = 10f;
                    taxNestedTable.AddCell(proffesionaltax);
                    PdfPCell proffesionaltaxamount = new PdfPCell(new Phrase(" 200.00"));
                    taxNestedTable.AddCell(proffesionaltaxamount);

                    PdfPCell lwp = new PdfPCell(new Phrase("Leave Without Pay"));
                    lwp.PaddingLeft = 10f;
                    taxNestedTable.AddCell(lwp);
                    taxNestedTable.AddCell(" " + deductionofleave.ToString());

                    PdfPCell nonwokingdaycolumn = new PdfPCell(new Phrase("Non Working Days"));
                    nonwokingdaycolumn.PaddingLeft = 10f;
                    nonwokingdaycolumn.PaddingBottom = 61;
                    taxNestedTable.AddCell(nonwokingdaycolumn);
                    PdfPCell nonwokingdaycolumnamount = new PdfPCell(new Phrase(" " + deductionofnonworkingdays.ToString()));
                    nonwokingdaycolumnamount.PaddingBottom = 61;
                    taxNestedTable.AddCell(nonwokingdaycolumnamount);

                    PdfPCell totalDeductions = new PdfPCell(new Phrase("Total Deductions", boldFont));
                   // totalDeductions.PaddingLeft = 10f;
                    taxNestedTable.AddCell(totalDeductions);
                    taxNestedTable.AddCell(" " + TotalamountDeductions.ToString());

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
                    PdfPCell totalsalary = new PdfPCell(new Phrase(" " + netSalaryInnumbers + "/-"));
                    totalsalary.PaddingLeft = 10f;
                    totalsalary.Colspan = 3;
                    table.AddCell(totalsalary);

                    PdfPCell salaryInwords = new PdfPCell(new Phrase("In Words", boldFont));
                    salaryInwords.PaddingLeft = 30f;
                    table.AddCell(salaryInwords);
                    PdfPCell totalsalaryInwords = new PdfPCell(new Phrase(" " + SalaryInWords + "Rupees only"));
                    totalsalaryInwords.PaddingLeft = 10f;
                    totalsalaryInwords.Colspan = 3;
                    table.AddCell(totalsalaryInwords);

                    PdfPCell note = new PdfPCell(new Phrase("* This is computer generated payslip and does not require any signature/company stamp.", italicFont));
                    note.PaddingLeft = 10f;
                    note.Colspan = 4;
                    table.AddCell(note);


                    doc.Add(table);
                    doc.Close();
                    if (arr[i].Type == "ZipFile")
                    {
                        //fileType = arr[i].Type;
                        fileName = string.Format("salaryslip of " + arr[i].FullName + ".pdf", i);
                        Stream memoryStreamForZipFile = new MemoryStream(memoryStream.ToArray());
                        memoryStreamForZipFile.Seek(0, SeekOrigin.Begin);
                        zipFile.AddEntry(fileName, memoryStreamForZipFile);
                    }
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
                        smtp.EnableSsl = false;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
                        smtp.Send(mail);
                    }
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                }
            }

            if (fileType == "ZipFile")
            {
                Response.Clear();
                Response.Buffer = false;
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "Slipdate of month " + Slipdate + ".zip"));
                zipFile.Save(Response.OutputStream);
                Response.End();
            }
            if(fileType == "SinglePdfDownload")
            {
                Response.ContentType = "pdf/application";
                Response.AddHeader("content-disposition", "attachment;filename=Salary Slip of " + fileName + ".pdf");
                Response.OutputStream.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
            }



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



        public JsonResult HolidaysDate(int Month, int Year)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Month", Month, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Year", Year, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var HolidayList = con.Query<SalarySlipViewModel>("sp_Holidays", parameters, commandType: CommandType.StoredProcedure);
            return Json(HolidayList, JsonRequestBehavior.AllowGet);
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


        public int CountNonWorkingDays(int Joinmonth, int JoinYear,string Date)
        {
            var firstDayOfMonth = new DateTime(JoinYear, Joinmonth, 1);
            var lastDayOfMonth = DateTime.Parse(Date);
            var nonWorkingDays = 0;
            while (firstDayOfMonth < lastDayOfMonth)
            {
                var weekday = firstDayOfMonth.DayOfWeek.ToString();
                if (weekday != "Saturday" && weekday != "Sunday")
                {
                    nonWorkingDays++;
                }
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }


            return nonWorkingDays;
        }


    }
}