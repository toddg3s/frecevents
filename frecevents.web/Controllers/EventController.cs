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
      EventInfoModel ei = Session["currentevent"] as EventInfoModel;

      if (IsPostBack() && ei != null)
      {
        if (Request.Form["action"] != null)
        {
          if (ei.Trailers)
          {
            ei.CurrentRegistration.TrailerSpace = int.Parse(Request["traileroption"]) *
                                                  int.Parse(Request["trailerspace"]);
          }
          else
          {
            ei.CurrentRegistration.TrailerSpace = 0;
          }
          if (ei.Lodging)
          {
            ei.CurrentRegistration.LodgingSpace = int.Parse(Request["lodgingoption"]) *
                                                  int.Parse(Request["lodgingspace"]);
          }
          else
          {
            ei.CurrentRegistration.LodgingSpace = 0;
          }
          ei.CurrentRegistration.FoodVolunteer = Request.Form["CurrentRegistration.FoodVolunteer"].Check2Bool();
          ei.CurrentRegistration.Notes = Request.Form["CurrentRegistration.Notes"];
          ei.CurrentRegistration.RegistrationRequest = 1;
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
          }
        }
        if (Request.Form["unregister"] != null)
        {
          Root.Data.Unregister(ei.CurrentRegistration.EventID, ei.CurrentRegistration.RiderID);
          ei.Registrations.Remove(ei.Registrations.First(reg => reg.EventID == ei.CurrentRegistration.EventID && reg.RiderID == ei.CurrentRegistration.RiderID));
          ei.CurrentRegistration = GetRegistrationOrDefault(ei.ID, ei.CurrentRegistration.RiderID);
        }
        if (Request.Form["newrider"] != null && Request.Form["newrider"] != "0")
        {
          var login = Root.Login;
          login.RiderID = Int32.Parse(Request.Form["newrider"]);
          Root.Login = login;
          ei.CurrentRegistration = GetRegistrationOrDefault(ei.ID, Root.Login.RiderID);
        }
      }
      else  // IsPostBack
      {
        ei = Root.Data.GetEvent(id);
        if (ei == null)
        {
          return View("EventNotFound", new ModelBase().Initialize());
        }
        ei.Initialize();
        var calevent = Calendar.GetEvent(id);
        ei.UpdateFromCalEvent(calevent);

        if (Root.Login != null && Root.Login.RiderID != 0)
        {
          ei.CurrentRegistration = GetRegistrationOrDefault(ei.ID, Root.Login.RiderID);
        }
        else
        {
          ei.CurrentRegistration = new RegistrationModel() { EventID = ei.ID };
        }
      }
      Session["currentevent"] = ei;
      return View(ei);
    }

    private RegistrationModel GetRegistrationOrDefault(string eventId, int riderId)
    {
      var reg = Root.Data.GetRegistration(eventId, riderId);
      if(reg==null)
      {
        var rider = Root.Data.GetRider(riderId);
        if(rider!=null)
        {
          reg = new RegistrationModel()
          {
            EventID = eventId,
            RiderID = riderId,
            TrailerSpace = rider.Trailerspace,
            LodgingSpace = rider.Lodgingspace,
            FoodVolunteer = rider.FoodVolunteer
          };
        }
      }
      return reg;
    }

    public ActionResult Upcoming()
    {
      return View(new ModelBase().Initialize());
    }

    public ActionResult List(string type)
    {
      return View(new ModelBase().Initialize());
    }

    public ActionResult Edit(string id)
    {
      if (Root.Login == null || Root.Login.UserType == LoginType.Member)
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
          Description = Request.Form["Description"],
          EventSite = Request.Form["EventSite"],
          EventType = Request.Form["EventType"],
          SiteURL = Request.Form["SiteURL"],
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

    public ActionResult Emails(string id, string type)
    {
      var ei = Root.Data.GetEvent(id);
      ei.Initialize();
      if (ei == null)
      {
        return View("EventNotFound", ModelBase.Default);
      }
      IEnumerable<string> emails;

      if (String.IsNullOrWhiteSpace(type))
      {
        emails = (from reg in ei.Registrations
                  join r in ei.Riders on reg.RiderID equals r.ID
                  select r.Email).Distinct();
      }
      else
      {
        switch (type.Trim().ToLower())
        {
          case "food":
            emails = (from reg in ei.Registrations
                      join r in ei.Riders on reg.RiderID equals r.ID
                      where reg.FoodVolunteer
                      select r.Email).Distinct();
            break;
          case "trailer":
            emails = (from reg in ei.Registrations
                      join r in ei.Riders on reg.RiderID equals r.ID
                      where reg.TrailerSpace != 0
                      select r.Email).Distinct();
            break;
          case "trailerneed":
            emails = (from reg in ei.Registrations
                      join r in ei.Riders on reg.RiderID equals r.ID
                      where reg.TrailerSpace < 0
                      select r.Email).Distinct();
            break;
          case "trailerhave":
            emails = (from reg in ei.Registrations
                      join r in ei.Riders on reg.RiderID equals r.ID
                      where reg.TrailerSpace > 0
                      select r.Email).Distinct();
            break;
          case "lodging":
            emails = (from reg in ei.Registrations
                      join r in ei.Riders on reg.RiderID equals r.ID
                      where reg.LodgingSpace != 0
                      select r.Email).Distinct();
            break;
          case "lodgingneed":
            emails = (from reg in ei.Registrations
                      join r in ei.Riders on reg.RiderID equals r.ID
                      where reg.LodgingSpace < 0
                      select r.Email).Distinct();
            break;
          case "lodginghave":
            emails = (from reg in ei.Registrations
                      join r in ei.Riders on reg.RiderID equals r.ID
                      where reg.LodgingSpace > 0
                      select r.Email).Distinct();
            break;
          default:
            emails = new string[] { };
            break;
        }
      }

      return Content(String.Join(";", emails));
    }
  }
}