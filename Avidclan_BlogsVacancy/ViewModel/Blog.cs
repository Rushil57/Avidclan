using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Avidclan_BlogsVacancy.ViewModel
{
    public class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string BlogType { get; set; }
        public string Image { get; set; }
        public DateTime PostingDate { get; set; }
        public string PostedBy { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        public string PageUrl { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string Faqs { get; set; }

        public List<BlogFaqs> BlogFaqs { get; set; }

        public string Questions { get; set; }

        public string Answers { get; set; }
    }

    public class BlogFaqs
    {
        public int Id { get; set; }
        public string Questions { get; set; }

        public string Answers { get; set; }

        public int BlogId { get; set; }
    }
}