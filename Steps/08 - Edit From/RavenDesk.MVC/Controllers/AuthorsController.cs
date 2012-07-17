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

        public ActionResult Edit(string id)
        {
            var vModel = AWorker.GenerateForm(id);
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
        public ActionResult AddUpatde(AuthorFormModel PostedData)
        {
            var results = AWorker.CommitChanges(PostedData);
            if(results.Success)
            {
                return View("InputSuccess", results);
            }

            PostedData.PopupTitle = "Edit Author - ERROR";
            results.PassErrorsToMvcModelState(ModelState);
            return View(PostedData);
        }


    }
}
