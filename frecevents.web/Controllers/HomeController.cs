using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using frecevents.web.Models;
using System.IO;

namespace frecevents.web.Controllers
{
    public class HomeController : Controller
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
            return Content("<html><body>Cache cleared</body></html>");
        }

        public ActionResult Upload()
        {
            UploadSpec model = Session["uploadspecs"] as UploadSpec;
            if(model==null)
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
            if((model.Mapping==null || model.Mapping.Count == 0) && Request["datatype"] != null)
            {
                if (Request["datatype"] == "events")
                {
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
                    model.Fields.AddRange(firstline.Split(separator));
                }
            }
            if(Request["mappingdone"]!=null)
            {
                foreach(var column in model.Mapping.Keys)
                {
                    if(!String.IsNullOrWhiteSpace(Request[column]) && Request[column] != "Select...")
                    {
                        model.Mapping[column] = Request[column];
                    }
                }
                var sr = new StringReader(model.Filedata);
                while(1==1)
                {
                    var dataline = sr.ReadLine();
                    if (dataline == null)
                        break;
                    var separator = (dataline.Contains('\t')) ? new char[] { '\t' } : new char[] { ',' };

                    var data = dataline.Split(separator);

                    switch(model.DataType)
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
                            break;
                    }
                }

            }
                Session["uploadspecs"] = model;
            return View(model);

        }

        private string GetDataValue(UploadSpec model, string[] data, string key)
        {
            if(!model.Mapping.ContainsKey(key))
            {
                return null;
            }
            var index = model.Fields.IndexOf(model.Mapping[key]);
            if(index < 0)
            {
                return null;
            }
            if(index >= data.Length)
            {
                return null;
            }
            return data[model.Fields.IndexOf(model.Mapping[key])];
        }
    }
}