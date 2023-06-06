using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using iTextSharp.text.pdf.qrcode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Avidclan_BlogsVacancy.Controllers
{
    public class LeaveController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public LeaveController()
        {
            con = new SqlConnection(connectionString);
        }
        // GET: Leave
        public ActionResult Index()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin", "BlogVacancy");
            }
            return View();
        }

        public JsonResult LeaveDates()
        {
            var UserId = Session["UserId"];
            var parameters = new DynamicParameters();
            parameters.Add("@Id", UserId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var GetLeaveDates = con.Query<LeaveDetailsViewModel>("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetLeaveDates.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaveDetails(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var GetLeaveDates = con.Query<LeaveViewModel>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetLeaveDates, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveStatus()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin", "BlogVacancy");
            }
            return View();
        }

        public JsonResult GetLeaves(string LeaveStatus)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveStatus", LeaveStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            var GetLeaveList = con.Query<LeaveViewModel>("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetLeaveList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TotalLeaves()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin", "BlogVacancy");
            }
            return View();
        }

        public ActionResult FeedBack()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin", "BlogVacancy");
            }
            return View();
        }

        public JsonResult GetTotalLeaveList()
        {
            var UserId = Session["UserId"];
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
            var LeaveList = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
            return Json(LeaveList, JsonRequestBehavior.AllowGet);
        }

        //
        public async Task<string> CheckTypeOfLeave(List<LeaveDetailsViewModel> Leaves, DateTime FromDate, int Id, object UserId, object JoinigDate, object ProbationPeriod)
        {
            try
            {
                List<LeaveDetailsViewModel> datas = new List<LeaveDetailsViewModel>();

                 var CurrentYear = FromDate.Year.ToString();
                var GivenMonth = FromDate.Month.ToString();

                var parameter = new DynamicParameters();
                parameter.Add("@Year", CurrentYear, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@LeaveDate", FromDate, DbType.Date, ParameterDirection.Input);
                //parameter.Add("@Month", GivenMonth, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                var ListOfLeaves = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure).ToList();
               
                var Attribute = new DynamicParameters();
                Attribute.Add("@LeaveId", Id, DbType.Int32, ParameterDirection.Input);
                Attribute.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                var ListLeaves = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", Attribute, commandType: CommandType.StoredProcedure).ToList();
               
                var Attributes = new DynamicParameters();
                var GetLastLeaveDate = ListLeaves.LastOrDefault();
                Attributes.Add("@LeaveDate", GetLastLeaveDate.LeaveDate, DbType.Date, ParameterDirection.Input);
                Attributes.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
                Attributes.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);
                var FutureListLeaves = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", Attributes, commandType: CommandType.StoredProcedure).ToList();
                if(FutureListLeaves.Count > 0)
                {
                    var pastPersonalLeave = 0;
                    var pastSickLeave = 0;
                    for (int i = 0; i< FutureListLeaves.Count; i++)
                    {
                        if(FutureListLeaves[i].PastLeave == "1")
                        {
                            if (FutureListLeaves[i].PersonalLeaves != null)
                            {
                                pastPersonalLeave++;
                            }
                            if(FutureListLeaves[i].SickLeaves != null)
                            {
                                pastSickLeave++;
                            }
                        }
                      
                        LeaveDetailsViewModel dataStore = new LeaveDetailsViewModel();
                        dataStore.LeaveDate = FutureListLeaves[i].LeaveDate;
                        dataStore.LeaveId = FutureListLeaves[i].LeaveId;
                        dataStore.Halfday = FutureListLeaves[i].Halfday;
                        dataStore.Id = FutureListLeaves[i].Id;
                        datas.Add(dataStore);


                        var parameters = new DynamicParameters();
                        parameters.Add("@Id", FutureListLeaves[i].Id, DbType.Int16, ParameterDirection.Input);
                        parameters.Add("@Mode", 15, DbType.Int32, ParameterDirection.Input);
                        var saveLeaves = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }

                    var Attribute1 = new DynamicParameters();
                    Attribute1.Add("@UserId",UserId, DbType.Int32, ParameterDirection.Input);
                    Attribute1.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
                    var PastListLeaves = con.Query<TypeOfLeave>("sp_PastLeaves", Attribute1, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (pastPersonalLeave != 0)
                    {
                        var totalpl = Convert.ToDouble(PastListLeaves.PersonalLeave) + pastPersonalLeave;
                        var parameters = new DynamicParameters();
                        parameters.Add("@PersonalLeave", totalpl, DbType.String, ParameterDirection.Input);
                        parameters.Add("@UserId", UserId, DbType.Int16, ParameterDirection.Input);
                        parameters.Add("@mode", 2, DbType.Int32, ParameterDirection.Input);
                        using (IDbConnection connection = new SqlConnection(connectionString))
                        {
                            var saveLeaves = connection.ExecuteScalar("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure);
                        }
                    }
                    if(pastSickLeave != 0)
                    {
                        var totalsl = Convert.ToDouble(PastListLeaves.SickLeave) + pastSickLeave;
                        var parameters = new DynamicParameters();
                        parameters.Add("@SickLeave", totalsl, DbType.String, ParameterDirection.Input);
                        parameters.Add("@UserId", UserId, DbType.Int16, ParameterDirection.Input);
                        parameters.Add("@mode", 3, DbType.Int32, ParameterDirection.Input);
                        using (IDbConnection connection = new SqlConnection(connectionString))
                        {
                            var saveLeaves = connection.ExecuteScalar("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure);
                        }
                    }
                }
                AddLeaves((Convert.ToDouble(ListOfLeaves[0].PersonalLeave)), (Convert.ToDouble(ListOfLeaves[0].SickLeave)), ListLeaves, ProbationPeriod, JoinigDate, UserId, FromDate);


                if(datas.Count > 0)
                {
                    var LeaveIds = datas.Select(x => x.LeaveId).Distinct().ToList();
                    foreach(var leaveid in LeaveIds)
                    {
                        var leavelist = datas.Where(x => x.LeaveId == leaveid).ToList();

                        var list = leavelist.FirstOrDefault();
                        var parameters = new DynamicParameters();
                        parameters.Add("@Year", CurrentYear, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@LeaveDate", list.LeaveDate, DbType.Date, ParameterDirection.Input);
                        parameters.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                        var Leaves1 = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure).ToList();

                        foreach (var item in leavelist)
                        {
                            var parameters1 = new DynamicParameters();
                            parameters1.Add("@LeaveDate", item.LeaveDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                            parameters1.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                            parameters1.Add("@LeaveId", item.LeaveId, DbType.Int64, ParameterDirection.Input);
                            parameters1.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                            var SaveLeaveDetails = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters1, commandType: CommandType.StoredProcedure);
                        }

                        var Attribute1 = new DynamicParameters();
                        Attribute1.Add("@LeaveId", leaveid, DbType.Int32, ParameterDirection.Input);
                        Attribute1.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                        ListLeaves = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", Attribute1, commandType: CommandType.StoredProcedure).ToList();


                        AddLeaves((Convert.ToDouble(Leaves1[0].PersonalLeave)),
                            (Convert.ToDouble(Leaves1[0].SickLeave)),
                            ListLeaves, ProbationPeriod, JoinigDate,
                            UserId, list.LeaveDate);
                    }

                   
                }
            }
            catch (Exception ex)
            {
                var Error = ex.InnerException + ex.StackTrace;
                return Error;
            }

            return "";
        }

        public bool AddLeaves(double personalLeave, double sickLeave, List<LeaveDetailsViewModel> ListLeaves, object ProbationPeriod, object JoinigDate, object UserId, DateTime FromDate)
        {
            var ProbationMonths = Convert.ToInt32(ProbationPeriod);
            var joiningDate = (DateTime)(JoinigDate);
            var JoiningDateAfterProbation = joiningDate.AddMonths(ProbationMonths);

            var TodaysDate = DateTime.Now;
            var DiffernceDate = Math.Abs((TodaysDate.Date - FromDate.Date).Days);

            var PersonalLeaves = personalLeave;
            var SickLeaves = sickLeave;

            bool sickLeavetaken = false;
            foreach (var leave in ListLeaves)
            {
                var monthDifference = (((leave.LeaveDate.Year - joiningDate.Year) * 12) +
                 leave.LeaveDate.Month - joiningDate.Month) + 1;
                if (monthDifference <= ProbationMonths)
                {
                    SetProbationLwpLeave(leave);
                }
                else
                {
                    if (leave.LeaveDate <= TodaysDate)
                    {
                        sickLeavetaken = true;
                        SickLeaves = SetSickorLwpLeave(leave, JoiningDateAfterProbation, SickLeaves, UserId);
                    }
                    if (ListLeaves.Count == 1 && leave.LeaveDate > TodaysDate)
                    {
                        if (DiffernceDate >= 7)
                        {
                            PersonalLeaves = SetPersonalorLwpLeave(leave, JoiningDateAfterProbation, PersonalLeaves, UserId);
                        }
                        else
                        {
                            sickLeavetaken = true;
                            SickLeaves = SetSickorLwpLeave(leave, JoiningDateAfterProbation, SickLeaves, UserId);
                        }
                    }
                    if (ListLeaves.Count > 1 && leave.LeaveDate > TodaysDate)
                    {
                        if (DiffernceDate >= 14)
                        {
                            PersonalLeaves = SetPersonalorLwpLeave(leave, JoiningDateAfterProbation, PersonalLeaves, UserId);
                        }
                        else
                        {
                            sickLeavetaken = true;
                            SickLeaves = SetSickorLwpLeave(leave, JoiningDateAfterProbation, SickLeaves, UserId);
                        }
                    }
                }
            }

            return sickLeavetaken;
        }
        public void SetProbationLwpLeave(LeaveDetailsViewModel leave)
        {
            var LwpLeaves = 1.0;
            var HalfDaySickLeave = 0.5;
            if (leave.Halfday == null)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Lwp", LwpLeaves, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
            }
            else
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Lwp", HalfDaySickLeave, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public double SetSickorLwpLeave(LeaveDetailsViewModel leavedates, object JoinigDate, double TotalSickLeave, object UserId)
        {
            bool PastLeave = CheckpastPersonalSickLeave(leavedates, UserId, "Sick");
            if (PastLeave)
            {
                return 0.0;
            }
            var LwpLeaves = 1.0;
            var HalfDaySickLeave = 0.5;
            var SickLeave = 1.0;
            var joiningDate = (DateTime)(JoinigDate);

            //var monthDifference = (((leavedates.LeaveDate.Year - joiningDate.Year) * 12) +
            //    leavedates.LeaveDate.Month - joiningDate.Month) + 1;

            //if (monthDifference >= 13)
            //{
            //    var yr = joiningDate.Month > DateTime.Now.Month ? DateTime.Now.Year - 1 : DateTime.Now.Year;
            //    var currentYearJoinDate = new DateTime(yr, joiningDate.Month, joiningDate.Day);
            //    monthDifference = (((leavedates.LeaveDate.Year - currentYearJoinDate.Year) * 12) +
            //    leavedates.LeaveDate.Month - currentYearJoinDate.Month) + 1;
            //}

            var monthDifference = leavedates.LeaveDate.Month;
            double SickLeaves = (double)monthDifference / (double)2;
            if (leavedates.Halfday != null)
            {
                if (TotalSickLeave < SickLeaves)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SickLeaves", HalfDaySickLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@mode", 11, DbType.Int32, ParameterDirection.Input);
                    var SaveSickLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Lwp", HalfDaySickLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                    var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }
                TotalSickLeave += 0.5;
            }
            else
            {
                double SickLeaveUsed = SickLeaves - TotalSickLeave;
                if (SickLeaveUsed == 0.5)
                {
                    SickLeave = 0.5;
                    var parameters = new DynamicParameters();
                    parameters.Add("@Lwp", SickLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                    var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }

                if (TotalSickLeave < SickLeaves)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SickLeaves", SickLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@mode", 11, DbType.Int32, ParameterDirection.Input);
                    var SaveSickLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);

                }
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Lwp", LwpLeaves, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                    var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }
                TotalSickLeave++;
            }

            return TotalSickLeave;

        }
        public double SetPersonalorLwpLeave(LeaveDetailsViewModel leave, object JoinigDate, double TotalPersonalLeave, object UserId)
        {
            bool PastLeave = CheckpastPersonalSickLeave(leave, UserId, "Personal");
            if (PastLeave)
            {
                return 0.0;
            }

            var LwpLeaves = 1;
            var HalfDayPersonalLeave = 0.5;
            var joiningDate = (DateTime)(JoinigDate);
            var PersonalLeave = 1.0;

            //var monthDifference = (((leave.LeaveDate.Year - joiningDate.Year) * 12) +
            //    leave.LeaveDate.Month - joiningDate.Month) + 1;
            //if (monthDifference >= 13)
            //{
            //    var yr = joiningDate.Month > DateTime.Now.Month ? DateTime.Now.Year - 1 : DateTime.Now.Year;
            //    var currentYearJoinDate = new DateTime(yr, joiningDate.Month, joiningDate.Day);
            //    monthDifference = (((leave.LeaveDate.Year - currentYearJoinDate.Year) * 12) +
            //    leave.LeaveDate.Month - currentYearJoinDate.Month) + 1;
            //}

            var monthDifference = leave.LeaveDate.Month;
            if (leave.Halfday != null)
            {
                if (TotalPersonalLeave < monthDifference)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@PersonalLeaves", HalfDayPersonalLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@mode", 9, DbType.Int32, ParameterDirection.Input);
                    var SavePersonalLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Lwp", HalfDayPersonalLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                    var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }
                TotalPersonalLeave += 0.5;
            }
            else
            {
                double PersonalLeaveUsed = monthDifference - TotalPersonalLeave;
                if (PersonalLeaveUsed == 0.5)
                {
                    PersonalLeave = 0.5;
                    var parameters = new DynamicParameters();
                    parameters.Add("@Lwp", PersonalLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                    var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }

                if (TotalPersonalLeave < monthDifference)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@PersonalLeaves", PersonalLeave, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@mode", 9, DbType.Int32, ParameterDirection.Input);
                    var SavePersonalLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Lwp", LwpLeaves, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@PastLeave", 0, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                    var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                }

                TotalPersonalLeave++;
            }

            return TotalPersonalLeave;

        }

        public bool CheckpastPersonalSickLeave(LeaveDetailsViewModel leave, object UserId, string flag)
        {
            var pastFulldayLeave = 1.0;
            bool PastLeave = false;
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var Pastleave = con.Query<TypeOfLeave>("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            if (Pastleave != null)
            {
                if (Pastleave.IsNoticePeriod)
                {
                    //PastLeave = true;
                    SetProbationLwpLeave(leave);
                    return true;
                }
                if (flag == "Personal")
                {
                    if (Convert.ToDouble(Pastleave.PersonalLeave) != 0.0 && Convert.ToDouble(Pastleave.PersonalLeave) != 0.5)
                    {
                        PastLeave = true;
                        if (leave.Halfday != null)
                        {
                            pastFulldayLeave = 0.5;
                        }
                        var parameter = new DynamicParameters();
                        parameter.Add("@PersonalLeaves", pastFulldayLeave, DbType.Decimal, ParameterDirection.Input);
                        parameter.Add("@PastLeave", 1, DbType.Boolean, ParameterDirection.Input);
                        parameter.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 9, DbType.Int32, ParameterDirection.Input);
                        var SavePersonalLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                    }
                    if (Convert.ToDouble(Pastleave.PersonalLeave) == 0.5 && leave.Halfday != null)
                    {
                        PastLeave = true;
                        pastFulldayLeave = 0.5;
                        var parameter = new DynamicParameters();
                        parameter.Add("@PersonalLeaves", pastFulldayLeave, DbType.Decimal, ParameterDirection.Input);
                        parameter.Add("@PastLeave", 1, DbType.String, ParameterDirection.Input);
                        parameter.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 9, DbType.Int32, ParameterDirection.Input);
                        var SavePersonalLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                    }
                    if (PastLeave)
                    {
                        var RemainingPersonalLeave = (Convert.ToDecimal(Pastleave.PersonalLeave)) - (Convert.ToDecimal(pastFulldayLeave));
                        var parameter = new DynamicParameters();
                        parameter.Add("@PersonalLeave", RemainingPersonalLeave, DbType.Decimal, ParameterDirection.Input);
                        parameter.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 2, DbType.Int32, ParameterDirection.Input);
                        var SavePersonalLeave = con.ExecuteScalar("sp_PastLeaves", parameter, commandType: CommandType.StoredProcedure);
                    }
                }
                if (flag == "Sick")
                {
                    if (Convert.ToDouble(Pastleave.SickLeave) != 0.0 && Convert.ToDouble(Pastleave.SickLeave) != 0.5)
                    {
                        PastLeave = true;
                        if (leave.Halfday != null)
                        {
                            pastFulldayLeave = 0.5;
                        }
                        var parameter = new DynamicParameters();
                        parameter.Add("@SickLeaves", pastFulldayLeave, DbType.Decimal, ParameterDirection.Input);
                        parameter.Add("@PastLeave", 1, DbType.Boolean, ParameterDirection.Input);
                        parameter.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 11, DbType.Int32, ParameterDirection.Input);
                        var SaveSickLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                    }
                    if (Convert.ToDouble(Pastleave.SickLeave) == 0.5 && leave.Halfday != null)
                    {
                        PastLeave = true;
                        pastFulldayLeave = 0.5;
                        var parameter = new DynamicParameters();
                        parameter.Add("@SickLeaves", pastFulldayLeave, DbType.Decimal, ParameterDirection.Input);
                        parameter.Add("@PastLeave", 1, DbType.String, ParameterDirection.Input);
                        parameter.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 11, DbType.Int32, ParameterDirection.Input);
                        var SaveSickLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                    }
                    if (PastLeave)
                    {
                        var RemainingSickLeave = (Convert.ToDecimal(Pastleave.SickLeave)) - (Convert.ToDecimal(pastFulldayLeave));
                        var parameter = new DynamicParameters();
                        parameter.Add("@SickLeave", RemainingSickLeave, DbType.Decimal, ParameterDirection.Input);
                        parameter.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                        parameter.Add("@mode", 3, DbType.Int32, ParameterDirection.Input);
                        var SaveRemainingLeave = con.ExecuteScalar("sp_PastLeaves", parameter, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            return PastLeave;
        }

        public JsonResult GetTotalLeaveBalanaceList()
        {
            var UserId = Session["UserId"];
            var Currentyear = DateTime.Now.Year.ToString();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Year", Currentyear, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 12, DbType.Int32, ParameterDirection.Input);
            var TotalLeaveList = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
            return Json(TotalLeaveList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalPastBalanaceList()
        {
            var UserId = Session["UserId"];
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var TotalPastList = con.Query<TypeOfLeave>("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure);
            return Json(TotalPastList, JsonRequestBehavior.AllowGet);
        }
    }
}

