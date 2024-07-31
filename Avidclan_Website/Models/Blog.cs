using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avidclan_Website.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortTitle { get; set; }
        public string BlogType { get; set; }
        public string Image { get; set; }
        public DateTime PostingDate { get; set; }
        public string PostedBy { get; set; }
        public int TotalRecords { get; set; }
        public string PageUrl { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string SchemaCode { get; set; }
        public string BlogPostingDate { get; set; }

        public string Questions { get; set; }
        public string Answers { get; set; }
    }
    public class BlogViewModel
    {
        public List<Blog> ListBlog { get; set; }
        public Pager pager { get; set; }
    }

    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            int _totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int _currentPage = page != null ? (int)page : 1;
            int _startPage = _currentPage - 5;
            int _endPage = _currentPage + 4;
            if (_startPage <= 0)
            {
                _endPage -= (_startPage - 1);
                _startPage = 1;
            }
            if (_endPage > _totalPages)
            {
                _endPage = _totalPages;
                if (_endPage > 10)
                {
                    _startPage = _endPage - 9;
                }
            }
            TotalItems = totalItems;
            CurrentPage = _currentPage;
            PageSize = pageSize;
            TotalPages = _totalPages;
            StartPage = _startPage;
            EndPage = _endPage;
        }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
    }


    public class BlogFaqs
    {
        public int Id { get; set; }
        public string Questions { get; set; }

        public string Answers { get; set; }

        public int BlogId { get; set; }
    }
}