using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDesk.Core.Data;

namespace RavenDesk.MVC.Controllers
{
    public class _BaseController : Controller
    {
        public DataContext Db { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Db = new DataContext();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            this.Db.Dispose();
            base.OnResultExecuted(filterContext);
        }
    }
}
