using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RavenDesk.MVC.Controllers
{
    public class HomeController : _BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
