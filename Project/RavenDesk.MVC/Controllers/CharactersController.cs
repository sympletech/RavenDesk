using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;
using RavenDesk.MVC.Models.Authors;
using RavenDesk.MVC.Models.Books;
using RavenDesk.MVC.Models.Characters;

namespace RavenDesk.MVC.Controllers
{
    public class CharactersController : _BaseController
    {
        protected CharacterWorker CWorker { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this.CWorker = new CharacterWorker(Db);
        }

        public ActionResult Index()
        {
            var vModel = CWorker.GenerateListView();
            return View(vModel);
        }

        [HttpPost]
        public ActionResult Search(CharacterListViewModel Params)
        {
            var vModel = CWorker.GenerateListView(Params);
            return View("Grid", vModel.Characters);
        }

        public ActionResult Edit(string cId)
        {
            var vModel = CWorker.GenerateForm(cId);
            vModel.PopupTitle = "Edit Character";
            return View("Form", vModel);
        }

        public ActionResult Add()
        {
            var vModel = CWorker.GenerateForm();
            vModel.PopupTitle = "Add New Character";
            return View("Form", vModel);
        }

        [HttpPost]
        public ActionResult AddUpatde(CharacterFormModel postedData)
        {
            bool isNew = postedData.Id == null;
            var results = CWorker.CommitChanges(postedData);
            if (results.Success)
            {
                if (isNew)
                {
                    return RedirectToAction("Edit", new {cId = postedData.Id});
                }
                return View("InputSuccess", results);
            }

            postedData.PopupTitle = "Edit Character - ERROR";
            results.PassErrorsToMvcModelState(ModelState);
            return View("Form", postedData);
        }

        [HttpPost]
        public ActionResult AddRelationship(string characterId, string realtedId)
        {
            var character = Character.Get(Db, characterId);

            var relatedObj = Db.Session.Load<dynamic>(realtedId);
            character.AddRelationship(relatedObj);

            var vModel = CWorker.GenerateForm(character.Id);
            return View("RelatedObjects", vModel);
        }

        [HttpPost]
        public ActionResult RemoveRelationship(string characterId, string realtedId)
        {
            var character = Author.Get(Db, characterId);
            var relatedObj = Db.Session.Load<dynamic>(realtedId);
            character.RemoveRelationship(relatedObj);

            var vModel = CWorker.GenerateForm(characterId);
            return View("RelatedObjects", vModel);
        }

        [HttpPost]
        public void Remove(string id)
        {
            var character = Character.Get(Db, id);
            character.Delete();
        }
    }
}
