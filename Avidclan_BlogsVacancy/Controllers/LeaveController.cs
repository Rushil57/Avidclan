using Avidclan_BlogsVacancy.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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
           // parameters.Add("@UserId", UserId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            var GetLeaveList = con.Query<LeaveViewModel>("sp_LeaveApplication", parameters, commandType: CommandType.StoredProcedure);
            return Json(GetLeaveList, JsonRequestBehavior.AllowGet);
        }
      
    }
}