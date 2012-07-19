using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;
using RavenDesk.Core.Models.Interfaces;
using RavenDesk.MVC.Helpers;
using RavenDesk.MVC.Models.Books;

namespace RavenDesk.MVC.Models.Characters
{
    public class CharacterWorker
    {
        public DataContext Db { get; set; }
        public CharacterWorker(DataContext db)
        {
            this.Db = db;
        }

        public CharacterListViewModel GenerateListView()
        {
            return GenerateListView(new CharacterListViewModel());
        }
        public CharacterListViewModel GenerateListView(CharacterListViewModel baseObject)
        {
            var vModel = baseObject;
            IQueryable<Character> items; 
                if(string.IsNullOrEmpty(baseObject.SearchTerm))
                {
                    items = Character.GetAll(Db);
                }else
                {
                    items = Character.Query(Db, x => x.Name == baseObject.SearchTerm);
                }
            {
                
            }
            vModel.Characters = new PageableSearchResults<ICharacter>
                                 {
                                    Items = items,
                                    CurPage = baseObject.PageNum,
                                    RecordsPerPage = 10
                                 };
            return vModel;
        }

        public CharacterFormModel GenerateForm()
        {
            var vModel = new CharacterFormModel()
                             {
                                 Authors = new List<IAuthor>(),
                                 Books = new List<IBook>()
                             };
            BuildFormDdLs(vModel);

            return vModel;
        }
        public CharacterFormModel GenerateForm(string id)
        {
            var baseObject = Character.Get(Db, id);
            Mapper.CreateMap<Character, CharacterFormModel>()
                .AfterMap((book, formModel) =>
                {
                    formModel.Authors = book.QueryRelatedObjects<Author>();
                    formModel.Books = book.QueryRelatedObjects<Book>();
                });
            var vModel = Mapper.Map<Character, CharacterFormModel>(baseObject);

            BuildFormDdLs(vModel);

            return vModel;
        }
        public void BuildFormDdLs(CharacterFormModel vModel)
        {
            var authorChoices = Author.GetAll(Db).ToList().Except(vModel.Authors);
            
            vModel.AuthorAssoiationOptions = ListToSelectList.ConvertToSelectList(
                authorChoices, "Id", "LastName", true, "Select Author To Add");

            var bookChoices = Book.GetAll(Db).ToList().Except(vModel.Books);

            vModel.BookAssoiationOptions = ListToSelectList.ConvertToSelectList(
                bookChoices, "Id", "Title", true, "Select Book To Add");


        }

        public DataObjectOperationResult CommitChanges(CharacterFormModel postedData)
        {
            Mapper.CreateMap<CharacterFormModel, Character>();
            var character = new Character(Db);
            Mapper.Map<CharacterFormModel, Character>(postedData, character);
            var results = character.Save();
            postedData.Id = character.Id;
            return results;
        }
    

    }
}