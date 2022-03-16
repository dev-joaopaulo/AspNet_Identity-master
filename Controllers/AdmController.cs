using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alura_AspNet_Identity.Controllers
{
    [Authorize]
    public class AdmController : Controller
    {
        // GET: Adm
        public ActionResult Index()
        {
            return View();
        }
    }
}