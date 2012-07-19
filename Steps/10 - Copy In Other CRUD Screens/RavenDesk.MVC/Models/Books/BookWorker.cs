using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;
using RavenDesk.Core.Models.Interfaces;
using RavenDesk.MVC.Helpers;
using RavenDesk.MVC.Models.Authors;

namespace RavenDesk.MVC.Models.Books
{
    public class BookWorker
    {
        public DataContext Db { get; set; }
        public BookWorker(DataContext db)
        {
            this.Db = db;
        }

        public BookListViewModel GenerateListView()
        {
            return GenerateListView(new BookListViewModel());
        }
        public BookListViewModel GenerateListView(BookListViewModel baseObject)
        {
            var vModel = baseObject;
            IQueryable<Book> items; 
                if(string.IsNullOrEmpty(baseObject.SearchTerm))
                {
                    items = Book.GetAll(Db);
                }else
                {
                    items = Book.Query(Db, x => x.Title == baseObject.SearchTerm);
                }
            {
                
            }
            vModel.Books = new PageableSearchResults<IBook>
                                 {
                                     Items = items,
                                    CurPage = baseObject.PageNum,
                                    RecordsPerPage = 10
                                 };
            return vModel;
        }

        public BookFormModel GenerateForm()
        {
            var vModel = new BookFormModel()
                             {
                                 Authors = new List<IAuthor>(),
                                 Characters = new List<ICharacter>()
                             };
            BuildFormDdLs(vModel);

            return vModel;
        }
        public BookFormModel GenerateForm(string id)
        {
            var baseObject = Book.Get(Db, id);
            Mapper.CreateMap<Book, BookFormModel>()
                .AfterMap((book, formModel) =>
                {
                    formModel.Authors = book.QueryRelatedObjects<Author>();
                    formModel.Characters = book.QueryRelatedObjects<Character>();
                });
            var vModel = Mapper.Map<Book, BookFormModel>(baseObject);

            BuildFormDdLs(vModel);

            return vModel;
        }
        public void BuildFormDdLs(BookFormModel vModel)
        {
            var authorChoices = Author.GetAll(Db).ToList().Except(vModel.Authors);
            
            vModel.AuthorAssoiationOptions = ListToSelectList.ConvertToSelectList(
                authorChoices, "Id", "LastName", true, "Select Author To Add");

            var characterChoices = Character.GetAll(Db).ToList().Except(vModel.Characters);

            vModel.CharacterAssoiationOptions = ListToSelectList.ConvertToSelectList(
                characterChoices, "Id", "Name", true, "Select Character To Add");
        }
    
        public DataObjectOperationResult CommitChanges(BookFormModel postedData)
        {
            Mapper.CreateMap<BookFormModel, Book>();
            var book = new Book(Db);
            Mapper.Map<BookFormModel, Book>(postedData, book);
            var results = book.Save();
            postedData.Id = book.Id;
            return results;
        }
    

    }
}