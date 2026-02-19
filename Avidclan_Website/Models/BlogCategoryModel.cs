using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_Website.Models
{
    public class BlogCategoryModel
    {
        public int Id { get; set; }
        public string BlogCategory { get; set; }
        public int OrderBy { get; set; }
    }
}