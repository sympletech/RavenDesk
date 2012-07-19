using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;
using RavenDesk.MVC.Models.Authors;

namespace RavenDesk.MVC.Controllers
{
    public class AuthorsController : _BaseController
    {
        protected AuthorWorker AWorker { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this.AWorker = new AuthorWorker(Db);
        }

        public ActionResult Index()
        {
            var vModel = AWorker.GenerateListView();
            return View(vModel);
        }

        [HttpPost]
        public ActionResult Search(AuthorListViewModel Params)
        {
            var vModel = AWorker.GenerateListView(Params);
            return View("Grid", vModel.Authors);
        }

        public ActionResult Edit(string aId)
        {
            var vModel = AWorker.GenerateForm(aId);
            vModel.PopupTitle = "Edit Author";
            return View("Form", vModel);
        }

        public ActionResult Add()
        {
            var vModel = AWorker.GenerateForm();
            vModel.PopupTitle = "Add New Author";
            return View("Form", vModel);
        }

        [HttpPost]
        public ActionResult AddUpatde(AuthorFormModel postedData)
        {
            bool isNew = postedData.Id == null;
            var results = AWorker.CommitChanges(postedData);
            if (results.Success)
            {
                if(isNew)
                {
                    return RedirectToAction("Edit", new {aID = postedData.Id});
                }
                return View("InputSuccess", results);
            }

            postedData.PopupTitle = "Edit Author - ERROR";
            results.PassErrorsToMvcModelState(ModelState);
            return View("Form", postedData);
        }

        [HttpPost]
        public ActionResult AddRelationship(string authorId, string realtedId)
        {
            var author = Author.Get(Db, authorId);
            var relatedObj = Db.Session.Load<dynamic>(realtedId);
            author.AddRelationship(relatedObj);

            var vModel = AWorker.GenerateForm(authorId);
            return View("RelatedObjects", vModel);
        }

        [HttpPost]
        public ActionResult RemoveRelationship(string authorId, string realtedId)
        {
            var author = Author.Get(Db, authorId);
            var relatedObj = Db.Session.Load<dynamic>(realtedId);
            author.RemoveRelationship(relatedObj);

            var vModel = AWorker.GenerateForm(authorId);
            return View("RelatedObjects", vModel);
        }

        [HttpPost]
        public void Remove(string id)
        {
            var author = Author.Get(Db, id);
            author.Delete();
        }
    }
}
