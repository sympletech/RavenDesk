using RavenDesk.Core.Models.Interfaces;
using RavenDesk.MVC.Helpers;

namespace RavenDesk.MVC.Models.Books
{
    public class BookListViewModel
    {
        public PageableSearchResults<IBook> Books { get; set; }

        public string SearchTerm { get; set; }

        public int PageNum { get; set; }
        public int TotalPages { get; set; }
    }
}