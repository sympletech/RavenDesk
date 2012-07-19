﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
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
        public AuthorListViewModel GenerateListView(AuthorListViewModel baseObject)
        {
            var vModel = baseObject;
            IQueryable<Author> items; 
                if(string.IsNullOrEmpty(baseObject.SearchTerm))
                {
                    items = Author.GetAll(Db);
                }else
                {
                    items = Author.Query(Db, x => x.LastName == baseObject.SearchTerm);
                }
                if(baseObject.OnlyShowLiving == true)
                {
                    items = items.Where(x => x.Alive == true);
                }
            {
                
            }
            vModel.Authors = new PageableSearchResults<IAuthor>
                                 {
                                     Items = items,
                                    CurPage = baseObject.PageNum,
                                    RecordsPerPage = 10
                                 };
            return vModel;
        }
    
        public AuthorFormModel GenerateForm()
        {
            var vModel = new AuthorFormModel();


            return vModel;
        }
        public AuthorFormModel GenerateForm(string id)
        {
            var baseObject = Author.Get(Db, id);
            Mapper.CreateMap<Author, AuthorFormModel>();
            var vModel = Mapper.Map<Author, AuthorFormModel>(baseObject);
            
            return vModel;
        }
    
        public DataObjectOperationResult CommitChanges(AuthorFormModel postedData)
        {
            Mapper.CreateMap<AuthorFormModel, Author>();
            var author = new Author(Db);
            Mapper.Map<AuthorFormModel, Author>(postedData, author);
            return author.Save();
        }
    

    }
}