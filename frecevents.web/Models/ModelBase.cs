using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace frecevents.web.Models
{
  public class ModelBase
  {
    public List<EventGroupModel> PastEvents { get; set; }
    public List<EventModel> UpcomingEvents { get; set; }
    public List<EventModel> TopUpcomingEvents { get; set; }

    public ModelBase()
    {
      PastEvents = new List<EventGroupModel>();
      UpcomingEvents = new List<EventModel>();
      TopUpcomingEvents = new List<EventModel>();
    }

    private static ModelBase s_default;
    public static ModelBase Default
    {
      get
      {
        if(s_default==null)
        {
          s_default = new ModelBase();
          s_default.Initialize();
        }
        return s_default;
      }
    }

    public virtual void Initialize()
    {
      var elist = GetEvents();
      PastEvents.Clear();
      var currgroup = new EventGroupModel();
      UpcomingEvents.Clear();
      var count = Int32.Parse(ConfigurationManager.AppSettings["TopUpcomingCount"] ?? "6");
      var index = 0;

      foreach (var e in elist.OrderBy(se => se.StartDateTime))
      {
        if (e.StartDateTime.Date < DateTime.Now.Date)
        {
          if (e.StartDateTime.Year.ToString() != currgroup.Name)
          {
            if (currgroup.Name != null)
            {
              PastEvents.Add(currgroup);
            }
            currgroup = new EventGroupModel() { Name = e.StartDateTime.Year.ToString() };
          }
          currgroup.Events.Add(e);
        }
        else
        {
          UpcomingEvents.Add(e);
          if (index++ < count)
          {
            TopUpcomingEvents.Add(e);
          }
        }
      }
      if (currgroup.Name != null)
      {
        PastEvents.Add(currgroup);
      }
    }

    protected List<EventModel> GetEvents()
    {
        List<EventModel> list = (List<EventModel>)Root.Cache.Get("eventlist");
      if (list == null)
      {
          freceventsEntities ctx = new freceventsEntities();
          list = Root.Data.GetEvents();
          Root.Cache.Set("eventlist", list);
      }
      return list;
    }
  }
}