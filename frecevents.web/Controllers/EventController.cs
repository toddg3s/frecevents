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
        public ActionResult Show(string id)
        {
          var ei = Root.Data.GetEvent(id);
          if(ei==null)
          {
              return View("EventNotFound",ModelBase.Default);
          }
          ei.Initialize();
          return View(ei);
        }

      public ActionResult Upcoming()
      {
        var mb = new ModelBase();
        mb.Initialize();
        return View(mb);
      }
    }
}