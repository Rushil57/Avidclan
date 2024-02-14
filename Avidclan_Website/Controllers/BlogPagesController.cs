using Avidclan_Website.Models;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Avidclan_Website.Controllers
{
    public class BlogPagesController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public BlogPagesController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: BlogPages
        public ActionResult Index(string url)
        {
            return View();
        }

        public ActionResult blog(string id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageUrl", id, DbType.String, ParameterDirection.Input);
            var CareerList = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            ViewBag.HtmlData = CareerList.Description;
            ViewBag.MetaData = CareerList.MetaDescription;
            ViewBag.MetaTitle = CareerList.MetaTitle;
            var CanonicalUrl = "https://www.avidclan.com/blog/" + id + "/";
            ViewBag.Url = CanonicalUrl;
            var schema = CreateFaqSchema(id);
            if(schema != "")
            {
                int index = schema.LastIndexOf(',');
                var schemadata = schema.Remove(index, 1);
                ViewBag.FaqSchema = schemadata;
            }
           
            return View();
        }

        public string CreateFaqSchema(string id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 10, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageUrl", id, DbType.String, ParameterDirection.Input);
            var blogList = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).ToList();
            StringBuilder sb = new StringBuilder();
            string blank_space = "      ";
            if (blogList != null && blogList.Count > 0)
            {
                sb.Append("<script type=\"application/ld+json\">\n");
                sb.Append(blank_space); sb.Append("{\n");
                sb.Append(blank_space); sb.Append("\"@context\": \"https://schema.org\",\n");
                sb.Append(blank_space); sb.Append("\"@type\": \"FAQPage\",\n");
                sb.Append(blank_space); sb.Append("\"mainEntity\": [\n");
                foreach (var blog in blogList)
                {
                    sb.Append(blank_space); sb.Append("{\n");
                    sb.Append(blank_space); sb.Append("\"@type\": \"Question\",\n");
                    sb.Append(blank_space); sb.Append("\"name\": \"" + blog.Questions + "\",\n");
                    sb.Append(blank_space); sb.Append("\"acceptedAnswer\": {\n");
                    sb.Append("           "); sb.Append(" \"@type\": \"Answer\",\n");
                    sb.Append("           "); sb.Append("\"text\": \"" + blog.Answers + "\"\n");
                    sb.Append(blank_space); sb.Append("}},\n");
                     //sb.Append("        "); sb.Append("}\n");
                }
                
                sb.Append(blank_space); sb.Append("]}\n");
                sb.Append(blank_space); sb.Append("</script>\n");
            }

            
            

            return sb.ToString();
        }
    }
}
