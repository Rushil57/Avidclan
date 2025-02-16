﻿using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf.qrcode;
using iTextSharp.tool.xml.html;
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

                //var Attributes = new DynamicParameters();
                //var GetLastLeaveDate = ListLeaves.LastOrDefault();
                //Attributes.Add("@LeaveDate", GetLastLeaveDate.LeaveDate, DbType.Date, ParameterDirection.Input);
                //Attributes.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
                //Attributes.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);
                //var FutureListLeaves = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", Attributes, commandType: CommandType.StoredProcedure).ToList();
                //if (FutureListLeaves.Count > 0)
                //{
                //    double pastPersonalLeave = 0.0, pastSickLeave = 0.0, currPersonalLeave = 0.0, currSickLeave = 0.0, compOff = 0.0;
                //    var updateParams = new List<DynamicParameters>();

                //    foreach (var leave in FutureListLeaves)
                //    {
                //        // Handle Past and Current Leave counts
                //        bool isPastLeave = leave.PastLeave == "1";

                //        if (leave.PersonalLeaves != null)
                //        {
                //            if (leave.Halfday != null)
                //            {
                //                if (isPastLeave) pastPersonalLeave += 0.5;
                //                else currPersonalLeave += 0.5;
                //            }
                //            else
                //            {
                //                if (isPastLeave) pastPersonalLeave++;
                //                else currPersonalLeave += Convert.ToDouble(leave.PersonalLeaves);
                //            }
                //        }

                //        if (leave.SickLeaves != null)
                //        {
                //            if (leave.Halfday != null)
                //            {
                //                if (isPastLeave) pastSickLeave += 0.5;
                //                else currSickLeave += 0.5;
                //            }
                //            else
                //            {
                //                if (isPastLeave) pastSickLeave++;
                //                else currSickLeave++;
                //            }
                //        }

                //        if(leave.CompOffLeave != null)
                //        {
                //            if (leave.Halfday != null)
                //            {
                //                compOff += 0.5;
                //            }
                //            else
                //            {
                //                compOff += Convert.ToDouble(leave.CompOffLeave);
                //            }
                //        }

                //        // Prepare data for LeaveDetailsViewModel
                //        dataStoreList.Add(new LeaveDetailsViewModel
                //        {
                //            LeaveDate = leave.LeaveDate,
                //            LeaveId = leave.LeaveId,
                //            Halfday = leave.Halfday,
                //            Id = leave.Id,
                //            CreatedDate = leave.CreatedDate
                //        });

                //        // Add parameters for updating individual leave
                //        var parameters = new DynamicParameters();
                //        parameters.Add("@Id", leave.Id, DbType.Int16, ParameterDirection.Input);
                //        parameters.Add("@Mode", 15, DbType.Int32, ParameterDirection.Input);
                //        updateParams.Add(parameters);
                //    }

                //    // Execute all individual leave updates in batch
                //    foreach (var param in updateParams)
                //    {
                //        con.ExecuteScalar("sp_LeaveApplicationDetails", param, commandType: CommandType.StoredProcedure);
                //    }


                //    // Get user leave balance
                //    var userParams = new DynamicParameters();
                //    userParams.Add("@Id", UserId, DbType.String, ParameterDirection.Input);
                //    userParams.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);

                //    var leaveBalance = con.Query<UserRegister>("sp_User", userParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
                //    if(leaveBalance != null)
                //    {
                //        if(currPersonalLeave != 0.0)
                //        {
                //            var updatedpl = currPersonalLeave + Convert.ToDouble(leaveBalance.PaidLeave);
                //            await UpdateLeaveBalance(UserId, Convert.ToDouble(updatedpl), Convert.ToDouble(leaveBalance.SickLeave));
                //        }
                //        if(currSickLeave != 0.0)
                //        {
                //            var updatesSl = currSickLeave + Convert.ToDouble(leaveBalance.SickLeave);
                //            await UpdateLeaveBalance(UserId, Convert.ToDouble(leaveBalance.PaidLeave), Convert.ToDouble(updatesSl));
                //        }
                //        if (compOff != 0.0)
                //        {
                //            var updateCompOff = compOff + Convert.ToDouble(leaveBalance.CompOffLeave);
                //            await UpdateUserCompOffBalance(UserId, updateCompOff);
                //        }
                //    }
                    
                //}

                await AddLeaves(ListLeaves, ProbationPeriod, JoinigDate, UserId, FromDate, wfhId);

                //if (dataStoreList.Count > 0)
                //{
                //    var LeaveIds = dataStoreList.Select(x => x.LeaveId).Distinct().ToList();
                //    foreach (var leaveid in LeaveIds)
                //    {
                //        var leavelist = dataStoreList.Where(x => x.LeaveId == leaveid).ToList();

                //        var list = leavelist.FirstOrDefault();

                //        foreach (var item in leavelist)
                //        {
                //            var parameters1 = new DynamicParameters();
                //            parameters1.Add("@LeaveDate", item.LeaveDate.ToShortDateString(), DbType.Date, ParameterDirection.Input);
                //            parameters1.Add("@Halfday", item.Halfday, DbType.String, ParameterDirection.Input);
                //            parameters1.Add("@LeaveId", item.LeaveId, DbType.Int64, ParameterDirection.Input);
                //            parameters1.Add("@CreatedDate", item.CreatedDate, DbType.DateTime, ParameterDirection.Input);
                //            parameters1.Add("@mode", 1, DbType.Int32, ParameterDirection.Input);
                //            var SaveLeaveDetails = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters1, commandType: CommandType.StoredProcedure);
                //        }

                //        var Attribute1 = new DynamicParameters();
                //        Attribute1.Add("@LeaveId", leaveid, DbType.Int32, ParameterDirection.Input);
                //        Attribute1.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                //        ListLeaves = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", Attribute1, commandType: CommandType.StoredProcedure).ToList();


                //        await AddLeaves(ListLeaves, ProbationPeriod, JoinigDate,UserId, list.LeaveDate, wfhId,true);
                //    }


                //}
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

                    bool isPreviousMonthLeave = leave.LeaveDate.Month < TodaysDate.Month && leave.LeaveDate.Year == TodaysDate.Year;
                    bool isWithinGracePeriod = TodaysDate.Day <= 7;

                    if (monthDifference <= ProbationMonths)
                    {
                        await SetProbationLwpLeave(leave);
                    }
                    else
                    {
                        if (isPreviousMonthLeave && !isWithinGracePeriod)
                        {
                            // If leave is from a previous month and today's date is past the 7th, mark it as LWP
                            //await SetPersonalorLwpLeave(leave, JoiningDateAfterProbation, UserId);
                            await SavePaidOrLwpLeave(leave.Id, leave.Halfday != null ? 0.5 : 1.0, "Lwp");
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
            }
            catch(Exception ex)
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
        public async Task UpdateLeaveBalance(object userId, double paidLeave, double sickLeave)
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
        public async Task<double> SetPersonalorLwpLeave(LeaveDetailsViewModel leave, object joiningDate, object userId, double? remainingLeaveToDeduct = null, bool skipCompOffCheck = false)
        {
            try
            {
                if (!skipCompOffCheck)
                {
                    bool iscompOffLeave = await CheckCompOffLeave(leave, joiningDate,userId);
                    if (iscompOffLeave)
                        return 0.0;
                }

                const double FULL_DAY_LEAVE = 1.0;
                const double HALF_DAY_LEAVE = 0.5;
                // Use remainingLeaveToDeduct if provided; otherwise, determine leaveTaken normally
                double leaveTaken = remainingLeaveToDeduct ?? (leave.Halfday != null ? HALF_DAY_LEAVE : FULL_DAY_LEAVE);

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

        #region compoffleave
        public async Task<bool> CheckCompOffLeave(LeaveDetailsViewModel leave, object joiningDate,object UserId)
        {
            bool CompOffLeave = false;
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            var compoffleave = con.Query<CompOffViewModel>("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure).ToList();

            try
            {
                const double FULL_DAY_LEAVE = 1.0;
                const double HALF_DAY_LEAVE = 0.5;
                double leaveTaken = leave.Halfday != null ? HALF_DAY_LEAVE : FULL_DAY_LEAVE;
                double remainingLeaveToDeduct = leaveTaken; // Total leave to be deducted
                double totalCompOffDeducted = 0; // Track total deducted comp-off leave

                if (compoffleave != null && compoffleave.Any())
                {
                    foreach (var compoff in compoffleave)
                    {
                        double availableDays = Convert.ToDouble(compoff.NumberOfDays);

                        if (availableDays > 0)
                        {
                            // Deduct the minimum required from the current record
                            double leaveToDeduct = Math.Min(remainingLeaveToDeduct, availableDays);

                            // Accumulate total deducted comp-off leave
                            totalCompOffDeducted += leaveToDeduct;

                            // Reduce remaining leave to deduct
                            remainingLeaveToDeduct -= leaveToDeduct;

                            // Update the comp-off balance
                            double updatedCompOffLeave = Math.Max(0, availableDays - leaveToDeduct);
                            await UpdateCompOffBalance(UserId, updatedCompOffLeave, compoff.Id);

                            // If all required leave has been deducted, exit loop
                            if (remainingLeaveToDeduct <= 0)
                                break;
                        }
                    }

                    // **Save the final total CompOff deduction**
                    if (totalCompOffDeducted > 0)
                    {
                        await SaveCompOffLeave(leave.Id, totalCompOffDeducted);
                        CompOffLeave = true;
                    }

                    // If leave is still remaining after all records are processed
                    if (remainingLeaveToDeduct > 0)
                    {
                        await SetPersonalorLwpLeave(leave, joiningDate, UserId, remainingLeaveToDeduct, true);
                        Console.WriteLine($"Remaining leave not covered by comp-off: {remainingLeaveToDeduct}");
                    }
                }
                else
                {
                    // Get user leave balance
                    var userParams = new DynamicParameters();
                    userParams.Add("@Id", UserId, DbType.String, ParameterDirection.Input);
                    userParams.Add("@Mode", 14, DbType.Int32, ParameterDirection.Input);

                    var TotalCompOffList = con.Query<UserRegister>("sp_User", userParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (TotalCompOffList != null)
                    {
                        var totalDays = TotalCompOffList.CompOffLeave;
                        if(Convert.ToDouble(totalDays) > 0)
                        {
                            // Deduct the minimum required from the current record
                            double leaveToDeduct = Math.Min(remainingLeaveToDeduct, Convert.ToDouble(totalDays));

                            // Save leave deduction record
                            await SaveCompOffLeave(leave.Id, leaveToDeduct);

                            // Update the comp-off balance
                            double updatedCompOffLeave = Math.Max(0, Convert.ToDouble(totalDays) - leaveToDeduct);
                            await UpdateUserCompOffBalance(UserId, updatedCompOffLeave);

                            // Mark that at least part of the leave has been deducted
                            CompOffLeave = true;

                            // Reduce remaining leave to deduct
                            remainingLeaveToDeduct -= leaveToDeduct;

                            // If leave is still remaining after all records are processed
                            if (remainingLeaveToDeduct > 0)
                            {
                                CompOffLeave = false;
                                await SetPersonalorLwpLeave(leave, joiningDate, UserId, remainingLeaveToDeduct, true);

                                Console.WriteLine($"Remaining leave not covered by comp-off: {remainingLeaveToDeduct}");
                                CompOffLeave = true;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - CheckCompOffLeave", ex.Message, ex.StackTrace);
            }

            return CompOffLeave;
        }

        private async Task SaveCompOffLeave(long leaveId, double leaveAmount)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompOffLeave", leaveAmount, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("@Id", leaveId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@mode", 19, DbType.Int32, ParameterDirection.Input);

            await con.ExecuteScalarAsync("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task UpdateCompOffBalance(object userId, double CompOffLeave, object CompOffId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", CompOffId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@NumberOfDays", CompOffLeave, DbType.String, ParameterDirection.Input);
            parameters.Add("@mode", 7, DbType.Int32, ParameterDirection.Input);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteScalarAsync("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // for userTable
        public async Task UpdateUserCompOffBalance(object userId, double CompOffLeave)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompOffLeave", CompOffLeave, DbType.String, ParameterDirection.Input);
            parameters.Add("@Id", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@mode", 17, DbType.Int32, ParameterDirection.Input);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteScalarAsync("sp_User", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        #endregion compoffleave
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
                await SaveTotalCompOffLeave(UserId);
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
            catch (Exception ex)
            {
                await ErrorLog("LeaveController - SaveSickAndPaidLeave", ex.Message, ex.StackTrace);
                return Json(new { message = ex.Message });
            }
        }

        public async Task<ActionResult> SaveTotalCompOffLeave(object UserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@mode", 8, DbType.Int32, ParameterDirection.Input);
                var TotalCompOffList = con.Query<CompOffViewModel>("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if(TotalCompOffList != null)
                {
                    TotalCompOffList.TotalDays = TotalCompOffList.TotalDays == null ? "0" : TotalCompOffList.TotalDays;
                    var compOffparameters = new DynamicParameters();
                    compOffparameters.Add("@CompOffLeave", TotalCompOffList.TotalDays, DbType.String, ParameterDirection.Input);
                    compOffparameters.Add("@Id", UserId, DbType.Int32, ParameterDirection.Input);
                    compOffparameters.Add("@mode", 17, DbType.Int32, ParameterDirection.Input);
                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.ExecuteScalarAsync("sp_User", compOffparameters, commandType: CommandType.StoredProcedure);
                        
                    }
                }
                return Json(new { message = "Saved Successfully" });
            }
            catch (Exception ex)
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


        public JsonResult GetWfhList(string wfhStatus)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@wfhStatus", wfhStatus, DbType.String, ParameterDirection.Input);
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

        public JsonResult GetEmployeeLeaveDetails(string Id)
        {
            var UserId = Session["UserId"];
            if(Id != null)
            {
                var splitId = Id.Split('_');
                var type = splitId[0];
                var typeId = splitId[1];

                if (type == "Leave")
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", typeId, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@Mode", 20, DbType.Int32, ParameterDirection.Input);

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
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", typeId, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@Mode", 21, DbType.Int32, ParameterDirection.Input);

                    using (var reader = con.QueryMultiple("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure))
                    {
                        // Read the data from the reader
                        var wfhdetaillist = reader.Read<LeaveViewModel>().ToList();
                        var leavelist = reader.Read<LeaveViewModel>().ToList();

                        // Prepare the dynamic object for JSON response
                        var dynamiclist = new
                        {
                            leavelist,
                            wfhdetaillist
                        };

                        return Json(dynamiclist, JsonRequestBehavior.AllowGet);
                    } // The reader is properly disposed of here
                }
               
            }
            return Json(null);

        }

        public async Task<string> DeleteWFHDetailsOfEmployee(int wfhDetailId)
        {
            try
            {
                // Step 1: Retrieve wfh details and check for date changes
                var wfhDetailsParameters = new DynamicParameters();
                wfhDetailsParameters.Add("@Id", wfhDetailId, DbType.Int16, ParameterDirection.Input);
                wfhDetailsParameters.Add("@Mode", 10, DbType.Int32, ParameterDirection.Input);

                var WFHResult = con.Query<LeaveViewModel>("sp_WorkFromHome", wfhDetailsParameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (WFHResult != null)
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

                    var WFH = con.Query<LeaveViewModel>("sp_WorkFromHome", wfhParameters, commandType: CommandType.StoredProcedure).ToList();

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

        public ActionResult CompOff()
        {
            return View();
        }

        public JsonResult GetComppOffDataList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var GetcommpoffList = con.Query<CompOffViewModel>("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetcommpoffList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompOfDetails(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            var GetcommpOffData = con.Query<CompOffViewModel>("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return Json(GetcommpOffData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalCompensationLeave()
        {
            var UserId = Session["UserId"];
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId, DbType.Int16, ParameterDirection.Input);
            parameters.Add("@Mode", 5, DbType.Double, ParameterDirection.Input);
            var GetcommpOffData = con.Query<CompOffViewModel>("sp_CompensationLeave", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetcommpOffData, JsonRequestBehavior.AllowGet);
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

