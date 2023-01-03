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
    }
}