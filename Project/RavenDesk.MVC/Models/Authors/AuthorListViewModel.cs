using System.Collections.Generic;
using RavenDesk.Core.Models;
using RavenDesk.Core.Models.Interfaces;

namespace RavenDesk.MVC.Models.Authors
{
    public class AuthorListViewModel
    {
        public IEnumerable<IAuthor> Authors { get; set; }

        public string SearchTerm { get; set; }
        
        public int PageNum { get; set; }
        public int TotalPages { get; set; }
    }
}