using Avidclan_Website.Models;
using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public async Task<ActionResult> blog(string id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageUrl", id, DbType.String, ParameterDirection.Input);
            var MetaDetails = con.Query<Blog>("sp_Blog", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            ViewBag.BlogId = MetaDetails.Id;
            //ViewBag.HtmlData = MetaDetails.Description;
            ViewBag.MetaData = MetaDetails.MetaDescription;
            ViewBag.MetaTitle = MetaDetails.MetaTitle;
            var CanonicalUrl = "https://www.avidclan.com/blog/" + id + "/";
            ViewBag.Url = CanonicalUrl;

            //CreateFAQs(MetaDetails.Id);

            string blogContent = MetaDetails.Description;

            // For Listing Table Content.
            var tableContentList = await CreateTableContent(blogContent);
            ViewBag.TableContent = tableContentList;

            string blank_space = "      ";
            var schemaCode = "";
            schemaCode = MetaDetails.SchemaCode;
            StringBuilder sb = new StringBuilder();
            if (schemaCode == null || schemaCode == "")
            {
                sb.Append("<script type=\"application/ld+json\">\n");
                sb.Append(blank_space); sb.Append("{\n");
                sb.Append(blank_space); sb.Append("\"@context\": \"https://schema.org\",\n");
                sb.Append(blank_space); sb.Append("\"@type\": \"BreadcrumbList\",\n");
                sb.Append(blank_space); sb.Append("\"itemListElement\": [\n");
                sb.Append(blank_space); sb.Append("{\n");
                sb.Append(blank_space); sb.Append("\"@type\": \"ListItem\",\n");
                sb.Append(blank_space); sb.Append("\"position\": \"1\",\n");
                sb.Append(blank_space); sb.Append("\"name\": \"Avidclan\",\n");
                sb.Append(blank_space); sb.Append("\"item\": \"https://www.avidclan.com/\"},\n");
                sb.Append(blank_space); sb.Append("{\n");
                sb.Append(blank_space); sb.Append("\"@type\": \"ListItem\",\n");
                sb.Append(blank_space); sb.Append("\"position\": \"2\",\n");
                sb.Append(blank_space); sb.Append("\"name\": \"Blog\",\n");
                sb.Append(blank_space); sb.Append("\"item\": \"https://www.avidclan.com/blog/\"},\n");
                sb.Append(blank_space); sb.Append("{\n");
                sb.Append(blank_space); sb.Append("\"@type\": \"ListItem\",\n");
                sb.Append(blank_space); sb.Append("\"position\": \"3\",\n");
                sb.Append(blank_space); sb.Append("\"name\": \"" + MetaDetails.MetaTitle + "\",\n");
                sb.Append(blank_space); sb.Append("\"item\": \"" + MetaDetails.MetaDescription + "\"}\n");

                sb.Append(blank_space); sb.Append("]}\n");
                sb.Append(blank_space); sb.Append("</script>\n");

                schemaCode = sb.ToString();
            }
            ViewBag.SchemaCode = schemaCode;

            return View();
        }

        public async void CreateFAQs(int blogId)
        {
            //get FAQ
            var sectionFAQ = "";
            var Dparameters = new DynamicParameters();
            Dparameters.Add("@Id", blogId, DbType.Int32, ParameterDirection.Input);
            Dparameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
            var reader = con.QueryMultiple("sp_Blog", Dparameters, commandType: CommandType.StoredProcedure);
            var bloglist = reader.Read<Blog>().ToList();
            var blogfaqslist = reader.Read<BlogFaqs>().ToList();
            
            if(blogfaqslist.Count > 0)
            {
                for(int i=0;  i< blogfaqslist.Count; i++)
                {
                    var count = i + 1;
                    sectionFAQ += "<div class=\"accordion-item\" style=\"margin-bottom: 20px\">" +
                                "<h2 class=\"accordion-header\" id=\"heading" + count + "\">" +
                                "<button class=\"accordion-button collapsed\" type=\"button\" data-bs-toggle=\"collapse\"" +
                                "data-bs-target=\"#collapse" + count + "\" aria-expanded=\"true\" aria-controls=\"collapse" + count + "\">" +
                                "" + blogfaqslist[i].Questions + "" +
                                "</button>" +
                                "</h2>" +
                                "<div id=\"collapse" + count + "\" class=\"accordion-collapse collapse\" aria-labelledby=\"headingOne\"" +
                                "data-bs-parent=\"#accordionExample\">" +
                                "<div class=\"accordion-body\">" +
                                blogfaqslist[i].Answers +
                                "</div>" +
                                "</div>" +
                                "</div>";
                }
                ViewBag.sectionFAQ = sectionFAQ;
            }
        }

        public async Task<string> CreateTableContent(string sb)
        {
            var keyPointsList = new List<KeyValuePair<string, string>>();
            string TableContent = "";
            string pattern = @"<h2[^>]*?>(?<TagText>.*?)</h2>";
            if(sb != null && sb.Length > 0)
            {
                foreach (Match match in Regex.Matches(sb, pattern, RegexOptions.IgnoreCase))
                {
                    //string value = match.Groups[1].Value.Trim();
                    string matchValue = match.Groups[1].Value.Trim();
                    string filteredString = Regex.Replace(matchValue, "<.*?>", "");
                    string simplifiedString = Regex.Replace(filteredString, "[^0-9A-Za-z _-]", "");
                    var dashedIdentity = simplifiedString.Replace(" ","-");
                    string listElement = "<li><a href=\"#"+ dashedIdentity + "\">"+ filteredString + "</a></li>";
                    TableContent += listElement;

                    var stringReplacment = string.Format("<h2 id=\"" + dashedIdentity + "\">" + match.Groups[1].Value.Trim() + "</h2>");
                    keyPointsList.Add(new KeyValuePair<string, string>(match.Value.Trim().ToString(),stringReplacment));
                }
                foreach (KeyValuePair<string, string> innerpair in keyPointsList)
                {
                    sb = sb.Replace(innerpair.Key, innerpair.Value);
                }
            }
            string BlogData = sb;
            ViewBag.HtmlData = BlogData;
            return TableContent;
        }
    }
}
