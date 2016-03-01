using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using frecevents.web.Models;

namespace frecevents.web.Controllers
{
    public class EventController : BaseController
    {
        public ActionResult Show(string id)
        {
          EventInfoModel ei = null;
          if(IsPostBack())
          {
            ei = Session["currentevent"] as EventInfoModel;

            if(Request.Form["action"]!=null)
            {
              ei.CurrentRegistration.TrailerSpace = int.Parse(Request["traileroption"])*
                                                    int.Parse(Request["trailerspace"]);
              ei.CurrentRegistration.Notes = Request["Notes"];
              if (Request["request"] != null)
              {
                if (ei.CurrentRegistration.RegistrationRequest != 2)
                {
                  ei.CurrentRegistration.RegistrationRequest = 1;
                }
              }
              else
              {
                ei.CurrentRegistration.RegistrationRequest = 3;
              }
            }
            if(Request.Form["unregister"]!=null)
            {
              Root.Data.Unregister(ei.CurrentRegistration.EventID, ei.CurrentRegistration.RiderID);
              var rider = ei.Riders.First(r => r.ID == ei.CurrentRegistration.RiderID);
              ei.Riders.Remove(rider);
              ei.CurrentRegistration = new RegistrationModel() { EventID = ei.ID, RiderID = ei.CurrentRegistration.RiderID };
            }
            if (Request.Form["newrider"] != null && Request.Form["newrider"] != "0")
            {
              ei.CurrentRegistration.RiderID = Int32.Parse(Request.Form["newrider"]);
            }
            Root.Data.Register(ei.CurrentRegistration);
          }
          else
          {
            ei = Root.Data.GetEvent(id);
            if (ei == null)
            {
              return View("EventNotFound", ModelBase.Default);
            }
            ei.Initialize();

            if (Root.Login != null && Root.Login.RiderID != 0)
            {
              ei.CurrentRegistration = Root.Data.GetRegistration(ei.ID, Root.Login.RiderID) ??
                                       new RegistrationModel() { EventID = ei.ID, RiderID = Root.Login.RiderID };
            }
            else
            {
              ei.CurrentRegistration = new RegistrationModel() { EventID = ei.ID };
            }
          }
          Session["currentevent"] = ei;
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
        return View(ModelBase.Default);
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