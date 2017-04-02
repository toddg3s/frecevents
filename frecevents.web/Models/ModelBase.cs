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
    public List<EventModel> AllEvents { get; set; }
    public List<RiderModel> Riders { get; set; }

    public ModelBase()
    {
      PastEvents = new List<EventGroupModel>();
      UpcomingEvents = new List<EventModel>();
      TopUpcomingEvents = new List<EventModel>();
      AllEvents = new List<EventModel>();
      Riders = new List<RiderModel>();
    }

    private static ModelBase s_default;
    public static ModelBase Default
    {
      get
      {
        if (s_default == null)
        {
          s_default = new ModelBase();
          s_default.Initialize();
        }
        return s_default;
      }
    }

    public virtual ModelBase Initialize()
    {
      var elist = GetEvents();
      PastEvents.Clear();
      var currgroup = new EventGroupModel();
      UpcomingEvents.Clear();
      var count = Int32.Parse(ConfigurationManager.AppSettings["TopUpcomingCount"] ?? "6");
      var index = 0;
      AllEvents.Clear();
      AllEvents.AddRange(elist);
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

      Riders = Root.Cache.Get("riderlist") as List<RiderModel>;
      if (Riders == null)
      {
        Riders = Root.Data.GetRiders();
        Root.Cache.Set("riderlist", Riders);
      }
      return this;
    }

    protected List<EventModel> GetEvents()
    {
      var list = Root.Cache.Get("eventlist") as List<EventModel>;
      if (list == null)
      {
        list = Root.Data.GetEvents();
        var cal = Calendar.GetEvents();
        foreach(var calevent in cal)
        {
          var ev = list.FirstOrDefault(e => e.ID == calevent.Id);
          if(ev==null)
          {
            var newev = EventInfoModel.FromCalEvent(calevent);
            Root.Data.SaveEvent(newev);
            list.Add(newev);
          }
          else
          {
            ev.Title = calevent.Summary;
            ev.StartDateTime = calevent.Start.DateTime ?? DateTime.Parse(calevent.Start.Date);
            ev.EndDateTime = calevent.End.DateTime ?? DateTime.Parse(calevent.End.Date);
          }
        }
        Root.Cache.Set("eventlist", list);
      }
      return list;
    }
  }
}