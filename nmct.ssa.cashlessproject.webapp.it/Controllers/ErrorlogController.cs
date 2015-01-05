using nmct.ba.cashlessproject.model.Management;
using nmct.ssa.cashlessproject.webapp.it.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nmct.ssa.cashlessproject.webapp.it.Controllers
{
    [Authorize]
    public class ErrorlogController : Controller
    {
        // GET: Errorlog
        public ActionResult Index()
        {
            List<Errorlog> errorlogs = new List<Errorlog>();
            errorlogs = ErrorlogDA.GetErrorlogs();

            return View("Index", errorlogs);
        }
    }
}