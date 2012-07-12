using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;
using RavenDesk.Core.Models.Interfaces;
using RavenDesk.MVC.Helpers;

namespace RavenDesk.MVC.Models.Authors
{
    public class AuthorWorker
    {
        public DataContext Db { get; set; }
        public AuthorWorker(DataContext db)
        {
            this.Db = db;
        }

        public AuthorListViewModel GenerateListView()
        {
            return GenerateListView(new AuthorListViewModel());
        }
        public AuthorListViewModel GenerateListView(AuthorListViewModel Params)
        {
            var vModel = Params;
            IQueryable<Author> items; 
                if(string.IsNullOrEmpty(Params.SearchTerm))
                {
                    items = Author.GetAll(Db);
                }else
                {
                    items = Author.Query(Db, x => x.LastName == Params.SearchTerm);
                }
                if(Params.OnlyShowLiving == true)
                {
                    items = items.Where(x => x.Alive == true);
                }
            {
                
            }
            vModel.Authors = new PageableSearchResults<IAuthor>
                                 {
                                     Items = items,
                                    CurPage = Params.PageNum,
                                    RecordsPerPage = 10
                                 };
            return vModel;
        }
    }
}