using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace frecevents.web.Models
{
  public class EventInfoModel : EventModel
  {
    public string EventType { get; set; }
    public string EventSite { get; set; }
    public string Description { get; set; }
    public string SiteURL { get; set; }
    public string SiteAddress { get; set; }
    public string MapUrl { get; set; }
    public string Notes { get; set; }

    internal string GenerateID()
    {
        var sb = new StringBuilder();
        if(!String.IsNullOrWhiteSpace(Description))
        {
            sb.Append(Firstbit(Description));
        }
        else if(!String.IsNullOrWhiteSpace(EventSite))
        {
            sb.Append(Firstbit(EventSite));
        }
        sb.AppendFormat("{0:mmyy}", StartDateTime);
        return sb.ToString();
    }

    internal string Firstbit(string value)
    {
        var firstpart = Description.Split(" ".ToCharArray())[0];
        if (firstpart.Length > 15)
        {
            firstpart = firstpart.Substring(0, 15);
        }
        return firstpart;
    }
    public EventInfo ToData()
    {
        return new EventInfo()
        {
            ID = ID,
            Title = Title,
            StartDateTime = StartDateTime,
            EndDateTime = EndDateTime,
            EventSite = EventSite,
            EventType = EventType,
            SiteAddress = SiteAddress,
            SiteURL = SiteURL,
            Description = Description,
            Notes = Notes,
            MapURL = MapUrl

        };
    }
    public static EventInfoModel FromData(EventInfo ei)
    {
        if (ei == null) return null;

        return new EventInfoModel()
                     {
                         ID = ei.ID,
                         Title = ei.Title,
                         StartDateTime = ei.StartDateTime,
                         EndDateTime = ei.EndDateTime,
                         EventSite = ei.EventSite,
                         EventType = ei.EventType,
                         SiteAddress = ei.SiteAddress,
                         SiteURL = ei.SiteURL,
                         Description = ei.Description,
                         Notes = ei.Notes,
                         MapUrl = ei.MapURL
                     };
    }
  }
}