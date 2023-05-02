using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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

        public async Task<string> CheckTypeOfLeave(List<LeaveDetailsViewModel> Leaves, DateTime FromDate, int Id, object UserId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter = new DynamicParameters();
                parameter.Add("@UserId", UserId, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                var ListOfLeaves = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameter, commandType: CommandType.StoredProcedure).ToList();

                var TodaysDate = DateTime.Now;
                var DiffernceDate = Math.Abs((TodaysDate.Date - FromDate.Date).Days);

                var GetCurrentmonthNumber = FromDate.Month;
                double SickLeaves = (double)FromDate.Month / (double)2;

                var Attribute = new DynamicParameters();
                Attribute = new DynamicParameters();
                Attribute.Add("@LeaveId", Id, DbType.Int32, ParameterDirection.Input);
                Attribute.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                var ListLeaves = con.Query<LeaveDetailsViewModel>("sp_LeaveApplicationDetails", Attribute, commandType: CommandType.StoredProcedure).ToList();

                if (FromDate <= TodaysDate)
                {
                    SetSickorLwpLeave(ListLeaves, (Convert.ToDouble(ListOfLeaves[0].SickLeave)), SickLeaves);
                    return "";
                }

                if (Leaves.Count == 1)
                {
                    if (DiffernceDate >= 7)
                    {
                        SetPersonalorLwpLeave(ListLeaves, (Convert.ToDouble(ListOfLeaves[0].PersonalLeave)), GetCurrentmonthNumber);
                    }
                    else
                    {
                        SetSickorLwpLeave(ListLeaves, (Convert.ToDouble(ListOfLeaves[0].SickLeave)), SickLeaves);
                    }
                }
                else
                {
                    if (DiffernceDate >= 14)
                    {
                        SetPersonalorLwpLeave(ListLeaves, (Convert.ToDouble(ListOfLeaves[0].PersonalLeave)), GetCurrentmonthNumber);
                    }
                    else
                    {
                        SetSickorLwpLeave(ListLeaves, (Convert.ToDouble(ListOfLeaves[0].SickLeave)), SickLeaves);
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

        public void SetPersonalorLwpLeave(List<LeaveDetailsViewModel> ListLeaves,double TotalPersonalLeave, int Month)
        {
            var PersonalLeave = 1;
            var LwpLeaves = 1;
            var HalfDayPersonalLeave = 0.5;
            foreach (var leave in ListLeaves)
            {
                var GetCurrentmonthNumber = leave.LeaveDate.Month;
                double SickLeaves = (double)GetCurrentmonthNumber / (double)2;
                if (leave.Halfday != null)
                {
                    if (TotalPersonalLeave < GetCurrentmonthNumber)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@PersonalLeaves", HalfDayPersonalLeave, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 9, DbType.Int32, ParameterDirection.Input);
                        var SavePersonalLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@Lwp", HalfDayPersonalLeave, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                        var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }
                    TotalPersonalLeave += 0.5;
                }
                else
                {
                    if (TotalPersonalLeave < GetCurrentmonthNumber)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@PersonalLeaves", PersonalLeave, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 9, DbType.Int32, ParameterDirection.Input);
                        var SavePersonalLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@Lwp", LwpLeaves, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leave.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                        var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }

                    TotalPersonalLeave++;
                }
               
            }
        }



        public void SetSickorLwpLeave(List<LeaveDetailsViewModel> ListLeaves, double TotalSickLeave, double sickLeaveRemaining)
        {
            var SickLeave = 1;
            var LwpLeaves = 1;
            var HalfDaySickLeave = 0.5;
            foreach (var leavedates in ListLeaves)
            {
                var GetCurrentmonthNumber = leavedates.LeaveDate.Month;
                double SickLeaves = (double)GetCurrentmonthNumber / (double)2;
                if (leavedates.Halfday != null)
                {
                    if (TotalSickLeave < SickLeaves)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@SickLeaves", HalfDaySickLeave, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 11, DbType.Int32, ParameterDirection.Input);
                        var SaveSickLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@Lwp", HalfDaySickLeave, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                        var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }
                    TotalSickLeave += 0.5;
                }
                else
                {
                    if (TotalSickLeave < SickLeaves)
                    {
                        //double SickLeaveused = SickLeaves - TotalSickLeave;
                        var parameters = new DynamicParameters();
                        parameters.Add("@SickLeaves", SickLeave, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 11, DbType.Int32, ParameterDirection.Input);
                        var SaveSickLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@Lwp", LwpLeaves, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("@Id", leavedates.Id, DbType.Int64, ParameterDirection.Input);
                        parameters.Add("@mode", 10, DbType.Int32, ParameterDirection.Input);
                        var SaveLwpLeave = con.ExecuteScalar("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
                    }
                    TotalSickLeave++;
                }
            }
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
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            var LeaveList = con.Query<TypeOfLeave>("sp_LeaveApplicationDetails", parameters, commandType: CommandType.StoredProcedure);
            return Json(LeaveList, JsonRequestBehavior.AllowGet);
        }
    }
    
}