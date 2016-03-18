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
              if (ei.Trailers)
              {
                ei.CurrentRegistration.TrailerSpace = int.Parse(Request["traileroption"])*
                                                      int.Parse(Request["trailerspace"]);
              }
              else
              {
                ei.CurrentRegistration.TrailerSpace = 0;
              }
              if (ei.Lodging)
              {
                ei.CurrentRegistration.LodgingSpace = int.Parse(Request["lodgingoption"])*
                                                      int.Parse(Request["lodgingspace"]);
              }
              else
              {
                ei.CurrentRegistration.LodgingSpace = 0;
              }
              ei.CurrentRegistration.FoodVolunteer = Request.Form["CurrentRegistration.FoodVolunteer"].Check2Bool();
              ei.CurrentRegistration.Notes = Request.Form["CurrentRegistration.Notes"];
              if (Request.Form["CurrentRegistration.Request"] != null && 
                Request.Form["CurrentRegistration.Request"].Check2Bool())
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
              Root.Data.Register(ei.CurrentRegistration);
              var reg =
                ei.Registrations.FirstOrDefault(
                  r => r.EventID == ei.CurrentRegistration.EventID && r.RiderID == ei.CurrentRegistration.RiderID);
              if (reg == null)
              {
                ei.Registrations.Add(ei.CurrentRegistration);
              }
              else
              {
                reg.TrailerSpace = ei.CurrentRegistration.TrailerSpace;
                reg.Notes = ei.CurrentRegistration.Notes;
                reg.RegistrationRequest = ei.CurrentRegistration.RegistrationRequest;
              }
            }
            if(Request.Form["unregister"]!=null)
            {
              Root.Data.Unregister(ei.CurrentRegistration.EventID, ei.CurrentRegistration.RiderID);
              ei.Registrations.Remove(ei.Registrations.First(reg => reg.EventID == ei.CurrentRegistration.EventID && reg.RiderID == ei.CurrentRegistration.RiderID));
              ei.CurrentRegistration = new RegistrationModel() { EventID = ei.ID, RiderID = ei.CurrentRegistration.RiderID };
            }
            if (Request.Form["newrider"] != null && Request.Form["newrider"] != "0")
            {
              var login = Root.Login;
              login.RiderID = Int32.Parse(Request.Form["newrider"]);
              Root.Login = login;
              ei.CurrentRegistration = Root.Data.GetRegistration(ei.ID, Root.Login.RiderID) ??
                                       new RegistrationModel() { EventID = ei.ID, RiderID = Root.Login.RiderID };
            }
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

        EventInfoModel eventinfo;
        if (IsPostBack())
        {
          eventinfo = new EventInfoModel()
          {
            ID = Request.Form["ID"],
            Title = Request.Form["Title"],
            Description =  Request.Form["Description"],
            EventSite = Request.Form["EventSite"],
            EventType = Request.Form["EventType"],
            SiteURL =  Request.Form["SiteURL"],
            MapUrl = Request.Form["MapUrl"],
            Notes = Request.Form["Notes"],
            Trailers = Request.Form["Trailers"].Check2Bool(),
            Lodging = Request.Form["Lodging"].Check2Bool()
          };
          DateTime dateval;
          if (!DateTime.TryParse(Request.Form["StartDateTime"], out dateval))
          {
            throw new Exception("Invalid date value passed for Start (" + Request.Form["StartDateTime"] + ")");
          }
          eventinfo.StartDateTime = dateval;
          if (!DateTime.TryParse(Request.Form["EndDateTime"], out dateval))
          {
            throw new Exception("Invalid date value passed for End (" + Request.Form["EndDateTime"] + ")");
          }
          eventinfo.EndDateTime = dateval;
          Root.Data.SaveEvent(eventinfo);

        }
        else
        {
          eventinfo = String.IsNullOrWhiteSpace(id) ? new EventInfoModel() : Root.Data.GetEvent(id);
        }
        eventinfo.Initialize();
        return View(eventinfo);
      }
    }
}