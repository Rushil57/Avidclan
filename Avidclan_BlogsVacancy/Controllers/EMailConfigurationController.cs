using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Avidclan_BlogsVacancy.Controllers
{
    public class EMailConfigurationController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public EMailConfigurationController()
        {
            con = new SqlConnection(connectionString);
        }
        // GET: EMailConfiguration
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EMailConfiguration()
        {
            if (Session["EmailId"] == null)
            {
                return RedirectToAction("UserLogin", "BlogVacancy");
            }
            return View();
        }


        [HttpPost]
        public ActionResult SaveConfiguration(EMailConfiguration eMailConfiguration)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromMail", eMailConfiguration.FromMail, DbType.String, ParameterDirection.Input);
            parameters.Add("@ToMail", eMailConfiguration.ToMail, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", eMailConfiguration.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@Host", eMailConfiguration.Host, DbType.String, ParameterDirection.Input);
            parameters.Add("@Port", eMailConfiguration.Port, DbType.String, ParameterDirection.Input);
            parameters.Add("@Id", eMailConfiguration.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            var resultConfiguration = con.Query<EMailConfiguration>("sp_EmailConfiguration", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            if (resultConfiguration == null)
            {
                resultConfiguration = new EMailConfiguration();
            }
            return Json(resultConfiguration, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ReadConfiguration()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var resultConfiguration = con.Query<EMailConfiguration>("sp_EmailConfiguration", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            if (resultConfiguration == null)
            {
                resultConfiguration = new EMailConfiguration();
            }
            return Json(resultConfiguration, JsonRequestBehavior.AllowGet);
        }
    }
    public class EMailConfiguration
    {
        public string FromMail { get; set; }
        public string ToMail { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Port { get; set;}
        public int Id { get; set; }
    }
}