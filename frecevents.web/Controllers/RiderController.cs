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
        // Save rider info
        Root.Data.SaveRider(rider);
        if (Root.Login != null)
        {
          Root.Login = new LoginInfo() { UserType = Root.Login.UserType, RiderID = rider.ID };
        }
        Root.Cache.Clear("riderlist");
        if(Request["return"]==null)
        {
          Response.Redirect("/riders");
          return View(rider);
        }
        else
        {
          Response.Redirect(Request["return"]);
          return View(rider);
        }
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