using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using frecevents.web.Models;

namespace frecevents.web.Controllers
{
    public class EventController : Controller
    {
        // GET: Event
        public ActionResult Show(string ID)
        {
            return View();
        }

      public ActionResult Upcoming()
      {
        var mb = new ModelBase();
        mb.Initialize();
        return View(mb);
      }
    }
}