﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using frecevents.web.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace frecevents.web.Controllers
{
  public class HomeController : BaseController
  {
    // GET: Home
    public ActionResult Index()
    {
      var model = new Home();
      model.Initialize();
      return View(model);
    }

    public ActionResult ClearCache()
    {
      Root.Cache.Clear("eventlist");
      Root.Cache.Clear("riderlist");
      return Content("<html><body>Cache cleared</body></html>");
    }

    public ActionResult Login()
    {
      if(Request.Form["site"]!=null)
      {
        var value = Regex.Replace(Request.Form["site"], "[^a-zA-Z]", "").ToLower();
        LoginType lt;

        switch(value)
        {
          case "moreleg":
          case "kickon":
            lt = LoginType.Member;
            break;
          case "heelsdown":
            lt = LoginType.Admin;
            break;
          default:
            return View(ModelBase.Default);
        }

        if(Root.Login!=null)
        {
          Root.Login = new LoginInfo() { UserType = lt, RiderID = Root.Login.RiderID };
        }
        else
        {
          Root.Login = new LoginInfo() { UserType = lt };
        }

        if(Request.Form["private"]!=null && Request.Cookies["logindata"]==null)
        {
          Response.Cookies.Add(new HttpCookie("logindata", Root.Login.ToString()) { Expires = DateTime.MaxValue });
        }

        if(Request.QueryString["return"]!=null)
        {
          Response.Redirect(Request.QueryString["return"]);
        }
        else
        {
          Response.Redirect("/Home/index");
        }
      }
      return View(ModelBase.Default);
    }

    public ActionResult Upload()
    {
      UploadSpec model = Session["uploadspecs"] as UploadSpec;
      if (model == null)
      {
        model = new UploadSpec();
        model.Initialize();
      }

      if (Request.Files.Count > 0)
      {
        var uploadedFile = Request.Files[0];
        byte[] buffer = new byte[uploadedFile.ContentLength];
        uploadedFile.InputStream.Read(buffer, 0, uploadedFile.ContentLength);
        var filedata = System.Text.ASCIIEncoding.ASCII.GetString(buffer);

        model.Filedata = filedata;
        model.Fileuploaded = true;
      }
      if ((model.Mapping == null || model.Mapping.Count == 0) && Request["datatype"] != null)
      {
        if (Request["datatype"] == "events")
        {
          model.DataType = "events";
          model.Mapping = new Dictionary<string, string>()
                    {
                        { "ID", ""},
                        { "Title", ""},
                        { "StartDateTime", ""},
                        { "EndDateTime", ""},
                        { "EventType", ""},
                        { "EventSite", ""},
                        { "Description", ""},
                        { "SiteURL", ""},
                        { "SiteAddress", ""},
                        { "MapURL", ""},
                        { "Notes", ""}
                    };
        }
        if (Request["datatype"] == "riders")
        {
          model.DataType = "riders";
          model.Mapping = new Dictionary<string, string>()
                    {
                        { "ID", ""},
                        { "Name", ""},
                        { "Email", ""},
                        { "Notes", ""}
                    };
        }
        model.Fields = new List<string>();
        var sr = new StringReader(model.Filedata);
        var firstline = sr.ReadLine();
        var separator = (firstline.Contains('\t')) ? new char[] { '\t' } : new char[] { ',' };

        if (Request["fieldnames"] == null)
        {
          var numfields = firstline.Split(separator).Length;
          for (var i = 1; i <= numfields; i++)
          {
            model.Fields.Add("Field" + i);
          }
          model.Filedata = String.Join(separator.ToString(), model.Fields) + "\n" + model.Filedata;
        }
        else
        {
          model.Headers = true;
          model.Fields.AddRange(firstline.Split(separator));
        }
      }
      if (Request["mappingdone"] != null)
      {
        var columns = model.Mapping.Keys.ToArray<string>();
        foreach (var column in columns)
        {
          if (!String.IsNullOrWhiteSpace(Request[column]) && Request[column] != "Select...")
          {
            model.Mapping[column] = Request[column];
          }
        }
        var sr = new StringReader(model.Filedata);
        sr.ReadLine();
        while (1 == 1)
        {
          var dataline = sr.ReadLine();
          if (dataline == null)
            break;
          var separator = (dataline.Contains('\t')) ? new char[] { '\t' } : new char[] { ',' };

          var data = dataline.Split(separator);
          for (var i = 0; i < data.Length; i++)
          {
            data[i] = data[i].Trim(' ', '"');
          }

          switch (model.DataType)
          {
            case "events":
              var ev = new EventInfoModel();
              ev.Title = GetDataValue(model, data, "Title");
              var value = GetDataValue(model, data, "StartDateTime");
              ev.StartDateTime = (value == null) ? DateTime.MinValue : DateTime.Parse(value);
              value = GetDataValue(model, data, "EndDateTime");
              ev.EndDateTime = (value == null) ? DateTime.MinValue : DateTime.Parse(value);
              ev.EventType = GetDataValue(model, data, "EventType") ?? "show";
              ev.EventSite = GetDataValue(model, data, "EventSite") ?? "";
              ev.SiteURL = GetDataValue(model, data, "SiteURL") ?? "";
              ev.SiteAddress = GetDataValue(model, data, "SiteAddress");
              ev.MapUrl = GetDataValue(model, data, "MapURL") ?? "";
              ev.Notes = GetDataValue(model, data, "Notes") ?? "";
              ev.ID = GetDataValue(model, data, "ID") ?? ev.GenerateID();
              Root.Data.SaveEvent(ev);
              break;
            case "riders":
              var rider = new RiderModel();
              rider.Name = GetDataValue(model, data, "Name");
              rider.Email = GetDataValue(model, data, "Email");
              var id = GetDataValue(model, data, "ID");
              if(!String.IsNullOrWhiteSpace(id))
              {
                rider.ID = Int32.Parse(id);
              }
              Root.Data.SaveRider(rider);
              break;
          }
        }

      }
      Session["uploadspecs"] = model;
      return View(model);

    }

    public ActionResult Error()
    {
      return View(ModelBase.Default);
    }

    public ActionResult Message()
    {
      var msg = new System.Net.Mail.MailMessage();
      msg.To.Add("admin@freedomrun.events");
      msg.From = new System.Net.Mail.MailAddress(
        String.IsNullOrWhiteSpace(Request.Form["email"]) ? "admin@freedomrun.events" : Request.Form["email"]
        );
      switch(Request.Form["source"])
      {
        case "error":
          msg.Subject = "Freedomrun.events Error report";
          break;
        default:
          msg.Subject = "Freedomrun.events General Feedback";
          break;
      }
      msg.Body = Request.Form["message"];
      SmtpClient smtp = new SmtpClient();
      smtp.Send(msg);
      return Content("success");
    }

    public ActionResult Help()
    {
      return View(new ModelBase().Initialize());
    }

    private string GetDataValue(UploadSpec model, string[] data, string key)
    {
      if (!model.Mapping.ContainsKey(key))
      {
        return null;
      }
      if (String.IsNullOrWhiteSpace(model.Mapping[key]))
      {
        return null;
      }
      var index = model.Fields.IndexOf(model.Mapping[key]) - 1;
      if (index < 0)
      {
        return null;
      }
      if (index >= data.Length)
      {
        return null;
      }
      return data[index];
    }
  }
}