﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using frecevents.web.Models;

namespace frecevents.web.Controllers
{
    public class EventController : BaseController
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

      public ActionResult List(string type)
      {
        return View();
      }

      public ActionResult Edit(string id)
      {
        if(Root.Login==null || Root.Login.UserType == LoginType.Member)
        {
          Response.Redirect("/Home/Login?return=" + Request.Path);
        }

        if(String.IsNullOrEmpty(id))
        {
          return Redirect("/Home/index");
        }
        if(IsPostBack())
        {

        }

        var eventinfo = Root.Data.GetEvent(id);
        return View(eventinfo);
      }
    }
}