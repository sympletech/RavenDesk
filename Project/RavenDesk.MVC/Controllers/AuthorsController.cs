using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDesk.Core.Models;
using RavenDesk.MVC.Models.Authors;

namespace RavenDesk.MVC.Controllers
{
    public class AuthorsController : _BaseController
    {
        public ActionResult Index()
        {
            var vModel = new AuthorListViewModel();
            vModel.Authors = Author.GetAll(Db);
            return View(vModel);
        }

    }
}
