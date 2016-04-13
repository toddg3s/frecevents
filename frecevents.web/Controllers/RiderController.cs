using frecevents.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace frecevents.web.Controllers
{
  public class RiderController : BaseController
  {
    // GET: Rider
    public ActionResult Index()
    {
      if (Root.Login == null)
      {
        Response.Redirect("/Home/Login?return=" + Request.Path);
      }

      return View(new ModelBase().Initialize());
    }

    public ActionResult edit(int id)
    {
      if (Root.Login == null)
      {
        Response.Redirect("/Home/Login?return=" + Request.Path);
      }

      if (IsPostBack())
      {
        var rider = new RiderModel();
        rider.ID = id;
        rider.Name = Request["Name"];
        rider.Email = Request["Email"];

        rider.Trailerspace = Int32.Parse(Request["traileroption"]) * Int32.Parse(Request["trailerspace"]);
        rider.Lodgingspace = Int32.Parse(Request["lodgingoption"]) * Int32.Parse(Request["lodgingspace"]);
        rider.FoodVolunteer = Request["FoodVolunteer"].Check2Bool();
        // Save rider info
        Root.Data.SaveRider(rider);

        var currevents = Request.Form.Keys.Cast<string>()
          .Where(name => name.StartsWith("currevent_") && !String.IsNullOrWhiteSpace(Request.Form[name]))
          .Select(name => name.Replace("currevent_", "")).ToList();
        var selectedevents = Request.Form.Keys.Cast<string>()
          .Where(name => name.StartsWith("selevent_") && Request.Form[name].Check2Bool())
          .Select(name => name.Replace("selevent_", "")).ToList();
        for (int i = 0; i < currevents.Count;i++)
        {
          if(selectedevents.Contains(currevents[i]))
          {
            selectedevents.Remove(currevents[i]);
            currevents.RemoveAt(i--);
          }
        }
        foreach(var eventid in currevents)
        {
          Root.Data.Unregister(eventid, rider.ID);
        }

        foreach(var eventid in selectedevents)
        {
          Root.Data.Register(new RegistrationModel()
          {
            EventID = eventid,
            RiderID = rider.ID,
            TrailerSpace = rider.Trailerspace,
            LodgingSpace = rider.Lodgingspace,
            FoodVolunteer = rider.FoodVolunteer,
            RegistrationRequest = 1
          });
        }

        if (Root.Login != null)
        {
          Root.Login = new LoginInfo() { UserType = Root.Login.UserType, RiderID = rider.ID };
        }
        Root.Cache.Clear("riderlist");


        Response.Redirect(Request["return"] ?? "/riders");
        return View(rider);
      }
      else
      {
        var rider = Root.Data.GetRider(id);
        if (rider == null)
        {
          if (id != 0)
          {
            return View("RiderNotFound", ModelBase.Default);
          }
          else
          {
            rider = new RiderModel();
          }
        }
        rider.Initialize();
        return View(rider);
      }
    }

    public ActionResult Emails()
    {
      var emails = ModelBase.Default.Riders.Select(r => r.Email).Distinct();
      return Content(String.Join(";", emails));
    }
  }
}