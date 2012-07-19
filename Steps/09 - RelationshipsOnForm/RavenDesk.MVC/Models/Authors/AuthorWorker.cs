using System;
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
            var vModel = new AuthorFormModel()
                             {
                                 Books = new List<IBook>(),
                                 Characters = new List<ICharacter>()
                             };
            BuildFormDdLs(vModel);

            return vModel;
        }
        public AuthorFormModel GenerateForm(string id)
        {
            var baseObject = Author.Get(Db, id);
            Mapper.CreateMap<Author, AuthorFormModel>()
                .AfterMap((author, formModel) =>
                {
                    formModel.Books = author.QueryRelatedObjects<Book>();
                    formModel.Characters = author.QueryRelatedObjects<Character>();
                });
            var vModel = Mapper.Map<Author, AuthorFormModel>(baseObject);

            BuildFormDdLs(vModel);

            return vModel;
        }
        public void BuildFormDdLs(AuthorFormModel vModel)
        {
            var bookChoices = Book.GetAll(Db).ToList().Except(vModel.Books);
            
            vModel.BookAssoiationOptions = ListToSelectList.ConvertToSelectList(
                bookChoices, "Id", "Title", true, "Select Book To Add");

            var characterChoices = Character.GetAll(Db).ToList().Except(vModel.Characters);

            vModel.CharacterAssoiationOptions = ListToSelectList.ConvertToSelectList(
                characterChoices, "Id", "Name", true, "Select Character To Add");
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