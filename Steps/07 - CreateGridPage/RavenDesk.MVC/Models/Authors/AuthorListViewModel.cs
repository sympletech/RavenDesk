using System.Collections.Generic;
using RavenDesk.Core.Models;
using RavenDesk.Core.Models.Interfaces;
using RavenDesk.MVC.Helpers;

namespace RavenDesk.MVC.Models.Authors
{
    public class AuthorListViewModel
    {
        public PageableSearchResults<IAuthor> Authors { get; set; }

        public string SearchTerm { get; set; }
        public bool OnlyShowLiving { get; set; }

        public int PageNum { get; set; }
        public int TotalPages { get; set; }
    }
}