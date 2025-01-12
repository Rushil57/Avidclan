using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using iTextSharp.text;
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
using System.Web.Helpers;
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
            if (Session["UserEmailId"] == null)
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
            //var GetLeaveDates = con.Query<LeaveDetailsViewModel>("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
            var reader = con.QueryMultiple("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
            var leavelist = reader.Read<LeaveDetailsViewModel>().ToList();
            var wfhdetaillist = reader.Read<WorkFromHomeViewModel>().ToList();
            var dynamiclist = new
            {
                leavelist = leavelist,
                wfhdetaillist = wfhdetaillist
            };
            return Json(dynamiclist, JsonRequestBehavior.AllowGet);
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
            if (Session["UserEmailId"] == null)
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
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin", "BlogVacancy");
            }
            return View();
        }

        public ActionResult FeedBack()
        {
            if (Session["UserEmailId"] == null)
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
        public async Task<string> CheckTypeOfLeave(List<LeaveDetailsViewModel> Leaves, DateTime FromDate, object Id, object UserId, object JoinigDate, object ProbationPeriod, object wfhId)
        {
            try
            {
                //List<LeaveDetailsViewModel> datas = new List<LeaveDetailsViewModel>();
                var dataStoreList = new List<LeaveDetailsViewModel>();

                var CurrentYear = FromDate.Year.ToString();
                var GivenMonth = FromDate.Month.ToString();

                //var parameter = new DynamicParameters();
                //parameter.Add("@Year", CurrentYear, DbType.Int32, ParameterDirection.Input);
                //parameter.Add("@LeaveDate", FromDate, DbType.Date, ParameterDirection.Input);
                ////parameter.Add("@Month", GivenMonth, DbType.Int32, ParameterDirection.Input);
                //parameter.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
                //parameter.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                //var ListOfLeaves = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure).ToList();

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
                if (FutureListLeaves.Count > 0)
                {
                    double pastPersonalLeave = 0.0, pastSickLeave = 0.0, currPersonalLeave = 0.0, currSickLeave = 0.0;
                    var updateParams = new List<DynamicParameters>();

                    foreach (var leave in FutureListLeaves)
                    {
                        // Handle Past and Current Leave counts
                        bool isPastLeave = leave.PastLeave == "1";

                        if (leave.PersonalLeaves != null)
                        {
                            if (leave.Halfday != null)
                            {
                                if (isPastLeave) pastPersonalLeave += 0.5;
                                else currPersonalLeave += 0.5;
                            }
                            else
                            {
                                if (isPastLeave) pastPersonalLeave++;
                                else currPersonalLeave++;
                            }
                        }

                        if (leave.SickLeaves != null)
                        {
                            if (leave.Halfday != null)
                            {
                                if (isPastLeave) pastSickLeave += 0.5;
                                else currSickLeave += 0.5;
                            }
                            else
                            {
                                if (isPastLeave) pastSickLeave++;
                                else currSickLeave++;
                            }
                        }

                        // Prepare data for LeaveDetailsViewModel
                        dataStoreList.Add(new LeaveDetailsViewModel
                        {
                            LeaveDate = leave.LeaveDate,
                            LeaveId = leave.LeaveId,
                            Halfday = leave.Halfday,
                            Id = leave.Id
                        });

                        // Add parameters for updating individual leave
                        var parameters = new DynamicParameters();
                        parameters.Add("@Id", leave.Id, DbType.Int16, ParameterDirection.Input);
                        parameters.Add("@Mode", 15, DbType.Int32, ParameterDirection.Input);
                        updateParams.Add(parameters);
                    }

                    // Execute all individual leave updates in batch
                    foreach (var param in updateParams)
                    {
                        con.ExecuteScalar("sp_LeaveApplicationDetails", param, commandType: CommandType.StoredProcedure);
                    }

                 
                    // Get user leave balance
                    var userParams = new DynamicParameters();
                    userParams.Add("@Id", UserId, DbType.String, ParameterDirection.Input);
                    userParams.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);

                    var leaveBalance = con.Query<UserRegister>("sp_User", userParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if(leaveBalance != null)
                    {
                        if(currPersonalLeave != 0.0)
                        {
                            var updatedpl = currPersonalLeave + Convert.ToDouble(leaveBalance.PaidLeave);
                            await UpdateLeaveBalance(UserId, Convert.ToDouble(updatedpl), Convert.ToDouble(leaveBalance.SickLeave));
                        }
                        else
                        {
                            var updatesSl = currSickLeave + Convert.ToDouble(leaveBalance.SickLeave);
                            await UpdateLeaveBalance(UserId, Convert.ToDouble(leaveBalance.PaidLeave), Convert.ToDouble(updatesSl));
                        }
                    }
                }

                await AddLeaves(ListLeaves, ProbationPeriod, JoinigDate, UserId, FromDate,wfhId);


                if (dataStoreList.Count > 0)
                {
                    var LeaveIds = dataStoreList.Select(x => x.LeaveId).Distinct().ToList();
                    foreach (var leaveid in LeaveIds)
                    {
                        var leavelist = dataStoreList.Where(x => x.LeaveId == leaveid).ToList();

                        var list = leavelist.FirstOrDefault();

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


                        await AddLeaves(ListLeaves, ProbationPeriod, JoinigDate,UserId, list.LeaveDate, wfhId);
                    }


                }
            }
            catch (Exception ex)
            {
                var Error = ex.InnerException + ex.StackTrace;
                await ErrorLog("LeaveController - CheckTypeOfLeave", ex.Message, ex.StackTrace);
                return Error;
            }

            return "";
        }

        public async Task<bool> AddLeaves(List<LeaveDetailsViewModel> ListLeaves, object ProbationPeriod, object JoinigDate, object UserId, DateTime FromDate, object wfhId)
        {
            bool sickLeavetaken = false;
            try
            {
                var ProbationMonths = Convert.ToInt32(ProbationPeriod);
                var joiningDate = Convert.ToDateTime(JoinigDate);

                var JoiningDateAfterProbation = joiningDate.AddMonths(ProbationMonths);

                var TodaysDate = DateTime.Now;
                

                DateTime fromDate = new DateTime();
                if (Convert.ToInt32(wfhId) != 0)
                {
                    var leaveParameters = new DynamicParameters();
                    leaveParameters.Add("@Id", wfhId, DbType.Int16, ParameterDirection.Input);
                    leaveParameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);

                    var fromdate = con.Query<LeaveViewModel>("sp_WorkFromHome", leaveParameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    fromDate = fromdate.Fromdate;
                }
                else
                {
                    fromDate = FromDate.Date;
                }
                var DiffernceDate = Math.Abs((TodaysDate.Date - fromDate).Days);

                foreach (var leave in ListLeaves)
                {
                    var monthDifference = (((leave.LeaveDate.Year - joiningDate.Year) * 12) +
                     leave.LeaveDate.Month - joiningDate.Month) + 1;
                    if (monthDifference <= ProbationMonths)
                    {
                        await SetProbationLwpLeave(leave);
                    }
                    else
                    {
                        if (leave.LeaveDate <= TodaysDate)
                        {
                            sickLeavetaken = true;
                            await SetSickorLwpLeave(leave, JoiningDateAfterProbation, UserId);
                        }
                        if (ListLeaves.Count == 1 && leave.LeaveDate > TodaysDate)
                        {
                            if (DiffernceDate >= 7)
                            {
                               await SetPersonalorLwpLeave(leave, JoiningDateAfterProbation, UserId);
                            }
                            else
                            {
                                sickLeavetaken = true;
                                await SetSickorLwpLeave(leave, JoiningDateAfterProbation, UserId);
                            }
                        }
                        if (ListLeaves.Count > 1 && leave.LeaveDate > TodaysDate)
                        {
                            if (DiffernceDate >= 14)
                            {
                               await SetPersonalorLwpLeave(leave, JoiningDateAfterProbation, UserId);
                            }
                            else
                            {
                                sickLeavetaken = true;
                               await SetSickorLwpLeave(leave, JoiningDateAfterProbation, UserId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - AddLeaves", ex.Message, ex.StackTrace);
            }

            return sickLeavetaken;
        }
        public async Task SetProbationLwpLeave(LeaveDetailsViewModel leave)
        {
            try
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
            }catch(Exception ex)
            {
                await ErrorLog("LeaveController - SetProbationLwpLeave", ex.Message, ex.StackTrace);

            }
        }

        #region sickLeave
        public async Task<double> SetSickorLwpLeave(LeaveDetailsViewModel leavedates, object joiningDate, object userId)
        {
            const double FULL_DAY_LEAVE = 1.0;
            const double HALF_DAY_LEAVE = 0.5;
            double sickLeave = FULL_DAY_LEAVE;

            try
            {
                // Check if this is a past sick leave
                //bool isPastLeave = await CheckpastPersonalSickLeave(leavedates, userId, "Sick");
                //if (isPastLeave)
                //    return 0.0;

                // Get user leave balance
                var userParams = new DynamicParameters();
                userParams.Add("@Id", userId, DbType.String, ParameterDirection.Input);
                userParams.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);

                var leaveBalance = con.Query<UserRegister>("sp_User", userParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
                double userTotalSickLeave = string.IsNullOrEmpty(leaveBalance?.SickLeave) ? 0 : Convert.ToDouble(leaveBalance.SickLeave);


                // Adjust sick leave value for half-day
                if (leavedates.Halfday != null)
                    sickLeave = HALF_DAY_LEAVE;

                if (leavedates.Halfday == null && userTotalSickLeave == HALF_DAY_LEAVE)
                {
                    // Handle the case when the balance is less than required leave
                    await SaveSickAndLwpLeave(leavedates.Id, HALF_DAY_LEAVE, HALF_DAY_LEAVE);
                    await UpdateLeaveBalance(userId, Convert.ToDouble(leaveBalance.PaidLeave), 0);
                }
                else if (userTotalSickLeave >= sickLeave)
                {
                    // Deduct leave from sick leave balance
                    await SaveLeave(leavedates.Id, sickLeave, "Sick");
                    double remainingSickLeave = Math.Max(0, userTotalSickLeave - sickLeave);
                    await UpdateLeaveBalance(userId, Convert.ToDouble(leaveBalance.PaidLeave), remainingSickLeave);
                }
                else
                {
                    // Deduct leave from LWP balance
                    await SaveLeave(leavedates.Id, sickLeave, "Lwp");
                }

                // Increment sick leave after application
                sickLeave += leavedates.Halfday != null ? HALF_DAY_LEAVE : FULL_DAY_LEAVE;
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - SetSickorLwpLeave", ex.Message, ex.StackTrace);
            }

            return sickLeave;
        }

        private async Task SaveSickAndLwpLeave(long leaveId, double sickLeave, double lwpLeave)
        {
            // Save sick leave
            var sickParams = new DynamicParameters();
            sickParams.Add("@SickLeaves", sickLeave, DbType.Decimal, ParameterDirection.Input);
            sickParams.Add("@Id", leaveId, DbType.Int64, ParameterDirection.Input);
            sickParams.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
            sickParams.Add("@mode", 11, DbType.Int32, ParameterDirection.Input);
            await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", sickParams, commandType: CommandType.StoredProcedure);

            // Save LWP leave
            var lwpParams = new DynamicParameters();
            lwpParams.Add("@Lwp", lwpLeave, DbType.Decimal, ParameterDirection.Input);
            lwpParams.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
            lwpParams.Add("@Id", leaveId, DbType.Int64, ParameterDirection.Input);
            lwpParams.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
            await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", lwpParams, commandType: CommandType.StoredProcedure);
        }

        private async Task SaveLeave(long leaveId, double leaveAmount, string leaveType)
        {
            var parameters = new DynamicParameters();
            parameters.Add(leaveType == "Sick" ? "@SickLeaves" : "@Lwp", leaveAmount, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("@Id", leaveId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@PastLeave", 0, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@mode", leaveType == "Sick" ? 11 : 10, DbType.Int32, ParameterDirection.Input);

            await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion  sickLeave
        private async Task UpdateLeaveBalance(object userId, double paidLeave, double sickLeave)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PaidLeave", paidLeave, DbType.String, ParameterDirection.Input);
            parameters.Add("@SickLeave", sickLeave, DbType.String, ParameterDirection.Input);
            parameters.Add("@mode", 13, DbType.Int32, ParameterDirection.Input);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteScalarAsync("sp_User", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        #region paidleave
        public async Task<double> SetPersonalorLwpLeave(LeaveDetailsViewModel leave, object joiningDate, object userId)
        {
            try
            {
                // Check if this is a past personal leave
                //bool isPastLeave = await CheckpastPersonalSickLeave(leave, userId, "Personal");
                //if (isPastLeave)
                //    return 0.0;

                const double FULL_DAY_LEAVE = 1.0;
                const double HALF_DAY_LEAVE = 0.5;
                double leaveTaken = leave.Halfday != null ? HALF_DAY_LEAVE : FULL_DAY_LEAVE;

                // Get user leave balance
                var userParams = new DynamicParameters();
                userParams.Add("@Id", userId, DbType.String, ParameterDirection.Input);
                userParams.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);

                var leaveBalance = con.Query<UserRegister>("sp_User", userParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
                double userTotalPaidLeave = string.IsNullOrEmpty(leaveBalance?.PaidLeave) ? 0 : Convert.ToDouble(leaveBalance.PaidLeave);
                double userTotalSickLeave = string.IsNullOrEmpty(leaveBalance?.SickLeave) ? 0 : Convert.ToDouble(leaveBalance.SickLeave);

                if (userTotalPaidLeave == HALF_DAY_LEAVE && leave.Halfday == null)
                {
                    // Split leave between personal and LWP
                    await SavePaidOrLwpLeave(leave.Id, HALF_DAY_LEAVE, "Personal");
                    await SavePaidOrLwpLeave(leave.Id, HALF_DAY_LEAVE, "Lwp");

                    // Update user leave balance
                    await UpdateLeaveBalance(userId, 0, userTotalSickLeave);
                }
                else if (userTotalPaidLeave >= leaveTaken)
                {
                    // Deduct personal leave
                    await SavePaidOrLwpLeave(leave.Id, leaveTaken, "Personal");

                    // Update leave balance after deduction
                    double remainingPaidLeave = Math.Max(0, userTotalPaidLeave - leaveTaken);
                    await UpdateLeaveBalance(userId, remainingPaidLeave, userTotalSickLeave);
                }
                else
                {
                    // Deduct from LWP if insufficient personal leave
                    await SavePaidOrLwpLeave(leave.Id, leaveTaken, "Lwp");
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - SetPersonalorLwpLeave", ex.Message, ex.StackTrace);
            }

            return 0;
        }

        private async Task SavePaidOrLwpLeave(long leaveId, double leaveAmount, string leaveType)
        {
            var parameters = new DynamicParameters();
            parameters.Add(leaveType == "Personal" ? "@PersonalLeaves" : "@Lwp", leaveAmount, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("@Id", leaveId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@PastLeave", 0, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("@mode", leaveType == "Personal" ? 9 : 10, DbType.Int32, ParameterDirection.Input);

            await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion  paidleave
        public async Task<bool> CheckpastPersonalSickLeave(LeaveDetailsViewModel leave, object UserId, string flag)
        {
            var pastFulldayLeave = 1.0;
            bool PastLeave = false;
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var Pastleave = con.Query<TypeOfLeave>("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            try {
                if (Pastleave != null)
                {
                    if (Pastleave.IsNoticePeriod)
                    {
                        //PastLeave = true;
                        await SetProbationLwpLeave(leave);
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
                            var SavePersonalLeave = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
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
                            var SavePersonalLeave = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                        }
                        if (PastLeave)
                        {
                            var RemainingPersonalLeave = (Convert.ToDecimal(Pastleave.PersonalLeave)) - (Convert.ToDecimal(pastFulldayLeave));
                            var parameter = new DynamicParameters();
                            parameter.Add("@PersonalLeave", RemainingPersonalLeave, DbType.Decimal, ParameterDirection.Input);
                            parameter.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                            parameter.Add("@mode", 2, DbType.Int32, ParameterDirection.Input);
                            var SavePersonalLeave = await con.ExecuteScalarAsync("sp_PastLeaves", parameter, commandType: CommandType.StoredProcedure);
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
                            var SaveSickLeave = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
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
                            var SaveSickLeave = await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure);
                        }
                        if (PastLeave)
                        {
                            var RemainingSickLeave = (Convert.ToDecimal(Pastleave.SickLeave)) - (Convert.ToDecimal(pastFulldayLeave));
                            var parameter = new DynamicParameters();
                            parameter.Add("@SickLeave", RemainingSickLeave, DbType.Decimal, ParameterDirection.Input);
                            parameter.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                            parameter.Add("@mode", 3, DbType.Int32, ParameterDirection.Input);
                            var SaveRemainingLeave = await con.ExecuteScalarAsync("sp_PastLeaves", parameter, commandType: CommandType.StoredProcedure);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - CheckpastPersonalSickLeave", ex.Message, ex.StackTrace);
            }

            return PastLeave;
        }

        public async Task<JsonResult> GetTotalLeaveBalanaceList()
        {
            try
            {
                var UserId = Session["UserId"];
                var Currentyear = DateTime.Now.Year.ToString();
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
                parameters.Add("@Year", Currentyear, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mode", 12, DbType.Int32, ParameterDirection.Input);
                var totalLeaveList = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                var breakparameters = new DynamicParameters();
                breakparameters.Add("@Id", UserId, DbType.String, ParameterDirection.Input);
                breakparameters.Add("@Mode", 15, DbType.Int32, ParameterDirection.Input);
                var userData = con.Query<UserRegister>("sp_User", breakparameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                return Json(new { TotalLeaveList = totalLeaveList, UserBreakData = userData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - GetTotalLeaveBalanaceList", ex.Message, ex.StackTrace);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetTotalPastBalanaceList()
        {
            try
            {
                var UserId = Session["UserId"];
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                var TotalPastList = con.Query<TypeOfLeave>("sp_PastLeaves", parameters, commandType: CommandType.StoredProcedure);
                return Json(TotalPastList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - GetTotalPastBalanaceList", ex.Message, ex.StackTrace);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangePassword()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("UserLogin", "BlogVacancy");
            }
            return View();
        }

        public async Task<JsonResult> CheckOldPassword()
        {
            try
            {
                var Id = Session["UserId"];
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@mode", 6, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var CheckPassword = connection.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);
                    return Json(CheckPassword, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - CheckOldPassword", ex.Message, ex.StackTrace);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task UpdatePassword(string Password)
        {
            try
            {
                var Id = Session["UserId"];
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@Password", Password, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", 7, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var UpdatePassword = connection.ExecuteScalar("sp_User", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - CheckOldPassword", ex.Message, ex.StackTrace);
            }
        }

        public async Task<ActionResult> SaveSickAndPaidLeave(UserRegister userRegister)
        {
            try
            {
                var UserId = Session["UserId"];
                var parameters = new DynamicParameters();
                parameters.Add("@Id", UserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@PaidLeave", userRegister.PaidLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@SickLeave", userRegister.SickLeave, DbType.String, ParameterDirection.Input);
                parameters.Add("@mode", 13, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    await connection.ExecuteScalarAsync("sp_User", parameters, commandType: CommandType.StoredProcedure);
                    return Json(new { message = "Saved Successfully" });
                }
            }
            catch(Exception ex)
            {
                await ErrorLog("LeaveController - SaveSickAndPaidLeave", ex.Message, ex.StackTrace);
                return Json(new { message = ex.Message });
            }
        }

        public async Task<ActionResult> GetUserLeaveBalance()
        {
            try
            {
                var UserId = Session["UserId"];
                var parameters = new DynamicParameters();
                parameters.Add("@Id", UserId, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);
                var Leavebalance = con.Query<UserRegister>("sp_User", parameters, commandType: CommandType.StoredProcedure);
                return Json(Leavebalance, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - SaveSickAndPaidLeave", ex.Message, ex.StackTrace);
                return Json(new { message = ex.Message });
            }
        }


        public JsonResult GetWfhList(string LeaveStatus)
        {
            var parameters = new DynamicParameters();
            //parameters.Add("@LeaveStatus", LeaveStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var GetwfhList = con.Query<LeaveViewModel>("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetwfhList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWFHDetails(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            var GetLeaveDates = con.Query<LeaveViewModel>("sp_WorkFromHome", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetLeaveDates, JsonRequestBehavior.AllowGet);
        }


        //DeleteLeaveDetailsApplication
        public async Task<string> DeleteLeaveDetailsApplication(int leaveApplicationId, int leaveId)
        {
            try
            {
                // Step 1: Delete leave record
                var deleteParameters = new DynamicParameters();
                deleteParameters.Add("@Id", leaveApplicationId, DbType.Int16, ParameterDirection.Input);
                deleteParameters.Add("@Mode", 15, DbType.Int32, ParameterDirection.Input);

                var deleteResult = con.ExecuteScalar("sp_LeaveApplicationDetails", deleteParameters, commandType: CommandType.StoredProcedure);

                // Step 2: Retrieve leave details and check for date changes
                var leaveParameters = new DynamicParameters();
                leaveParameters.Add("@Id", leaveId, DbType.Int16, ParameterDirection.Input);
                leaveParameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);

                var leaveDates = con.Query<LeaveViewModel>("sp_LeaveApplicationDetails", leaveParameters, commandType: CommandType.StoredProcedure).ToList();

                if (leaveDates != null && leaveDates.Any())
                {
                    var fromDate = leaveDates[0].Fromdate;
                    var toDate = leaveDates[0].Todate;
                    var firstLeaveDate = leaveDates.Select(x => x.LeaveDates).FirstOrDefault();
                    var lastLeaveDate = leaveDates.Select(x => x.LeaveDates).LastOrDefault();

                    // Step 3: Update dates if there's a change
                    if (fromDate != firstLeaveDate || toDate != lastLeaveDate)
                    {
                        var updateParameters = new DynamicParameters();
                        updateParameters.Add("@Fromdate", firstLeaveDate, DbType.Date, ParameterDirection.Input);
                        updateParameters.Add("@Todate", lastLeaveDate, DbType.Date, ParameterDirection.Input);
                        updateParameters.Add("@Id", leaveId, DbType.Int16, ParameterDirection.Input);
                        updateParameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);

                        var updateResult = con.ExecuteScalar("sp_LeaveApplication", updateParameters, commandType: CommandType.StoredProcedure);
                    }
                }

                return "Deleted Successfully";
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - DeleteLeaveDetailsApplication", ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public JsonResult GetEmployeeLeaveDetails(int Id)
        {
            var UserId = Session["UserId"];
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@Mode", 17, DbType.Int32, ParameterDirection.Input);

            using (var reader = con.QueryMultiple("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure))
            {
                // Read the data from the reader
                var leavelist = reader.Read<LeaveViewModel>().ToList();
                var wfhdetaillist = reader.Read<LeaveViewModel>().ToList();

                // Prepare the dynamic object for JSON response
                var dynamiclist = new
                {
                    leavelist,
                    wfhdetaillist
                };

                return Json(dynamiclist, JsonRequestBehavior.AllowGet);
            } // The reader is properly disposed of here
        }

        //DeleteWFHDetailsOfEmployee
        public async Task<string> DeleteWFHDetailsOfEmployee(int wfhDetailId)
        {
            try
            {
                // Step 1: Retrieve wfh details and check for date changes
                var wfhDetailsParameters = new DynamicParameters();
                wfhDetailsParameters.Add("@Id", wfhDetailId, DbType.Int16, ParameterDirection.Input);
                wfhDetailsParameters.Add("@Mode", 10, DbType.Int32, ParameterDirection.Input);

                var WFHResult = con.Query<LeaveViewModel>("sp_WorkFromHome", wfhDetailsParameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if(WFHResult != null)
                {
                    // dlete record
                    var deleteParameters = new DynamicParameters();
                    deleteParameters.Add("@Id", wfhDetailId, DbType.Int16, ParameterDirection.Input);
                    deleteParameters.Add("@Mode", 11, DbType.Int32, ParameterDirection.Input);

                    var deleteResult = con.ExecuteScalar("sp_WorkFromHome", deleteParameters, commandType: CommandType.StoredProcedure);

                    var wfhId = WFHResult.WFHId;

                    // retrive work from home check.
                    var wfhParameters = new DynamicParameters();
                    wfhParameters.Add("@Id", wfhId, DbType.Int16, ParameterDirection.Input);
                    wfhParameters.Add("@Mode", 12, DbType.Int32, ParameterDirection.Input);

                    var WFH= con.Query<LeaveViewModel>("sp_WorkFromHome", wfhParameters, commandType: CommandType.StoredProcedure).ToList();

                    var firstDate = WFH[0].Fromdate;
                    var lastDate = WFH[0].Todate;

                    var UpdatedFirstDate = WFH.Select(x => x.WFHDates).FirstOrDefault();
                    var UpdatedLastdate = WFH.Select(x => x.WFHDates).LastOrDefault();
                    // when there is only one record of wfh delete everything
                    if (UpdatedFirstDate != firstDate || UpdatedLastdate != lastDate)
                    {
                        var updateParameters = new DynamicParameters();
                        updateParameters.Add("@Fromdate", UpdatedFirstDate, DbType.Date, ParameterDirection.Input);
                        updateParameters.Add("@Todate", UpdatedLastdate, DbType.Date, ParameterDirection.Input);
                        updateParameters.Add("@Id", wfhId, DbType.Int16, ParameterDirection.Input);
                        updateParameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);

                        var updateResult = con.ExecuteScalar("sp_WorkFromHome", updateParameters, commandType: CommandType.StoredProcedure);
                    }
                    

                }
                return "Deleted Successfully";
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - DeleteLeaveDetailsApplication", ex.Message, ex.StackTrace);
                return ex.Message;
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

