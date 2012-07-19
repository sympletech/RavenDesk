using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDesk.Core.Data;
using RavenDesk.Core.Models;
using RavenDesk.MVC.Models.Authors;
using RavenDesk.MVC.Models.Books;

namespace RavenDesk.MVC.Controllers
{
    public class BooksController : _BaseController
    {
        protected BookWorker BWorker { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this.BWorker = new BookWorker(Db);
        }

        public ActionResult Index()
        {
            var vModel = BWorker.GenerateListView();
            return View(vModel);
        }

        [HttpPost]
        public ActionResult Search(BookListViewModel Params)
        {
            var vModel = BWorker.GenerateListView(Params);
            return View("Grid", vModel.Books);
        }

        public ActionResult Edit(string bId)
        {
            var vModel = BWorker.GenerateForm(bId);
            vModel.PopupTitle = "Edit Book";
            return View("Form", vModel);
        }

        public ActionResult Add()
        {
            var vModel = BWorker.GenerateForm();
            vModel.PopupTitle = "Add New Book";
            return View("Form", vModel);
        }

        [HttpPost]
        public ActionResult AddUpatde(BookFormModel postedData)
        {
            bool isNew = postedData.Id == null;
            var results = BWorker.CommitChanges(postedData);
            if (results.Success)
            {
                if(isNew)
                {
                    return RedirectToAction("Edit", new {bId = postedData.Id});
                }
                return View("InputSuccess", results);
            }

            postedData.PopupTitle = "Edit Book - ERROR";
            results.PassErrorsToMvcModelState(ModelState);
            return View("Form", postedData);
        }

        [HttpPost]
        public ActionResult AddRelationship(string bookId, string realtedId)
        {
            var book = Book.Get(Db, bookId);

            var relatedObj = Db.Session.Load<dynamic>(realtedId);
            book.AddRelationship(relatedObj);

            var vModel = BWorker.GenerateForm(book.Id);
            return View("RelatedObjects", vModel);
        }

        [HttpPost]
        public ActionResult RemoveRelationship(string bookId, string realtedId)
        {
            var author = Author.Get(Db, bookId);
            var relatedObj = Db.Session.Load<dynamic>(realtedId);
            author.RemoveRelationship(relatedObj);

            var vModel = BWorker.GenerateForm(bookId);
            return View("RelatedObjects", vModel);
        }

        [HttpPost]
        public void Remove(string id)
        {
            var book = Book.Get(Db, id);
            book.Delete();
        }
    }
}
