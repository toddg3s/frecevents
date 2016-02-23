using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace frecevents.web.Models
{
  public class EventModel
  {
    public string ID { get; set; }
    public string Title { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    public static EventModel FromData(Event eventData)
    {
        return new EventModel() { ID = eventData.ID, Title = eventData.Title, StartDateTime = eventData.StartDateTime, EndDateTime = eventData.EndDateTime };
    }
    public virtual string FormatDateRange(DateFormat format)
    {
      switch (format)
      {
        case DateFormat.StartShort:
          if (StartDateTime.Year == DateTime.Now.Year)
          {
            return StartDateTime.ToString("MMM d");
          }
          else
          {
            return StartDateTime.ToString("MMM d yyyy");
          }
        case DateFormat.EndShort:
          if (EndDateTime == DateTime.MinValue)
          {
            return "";
          }
          if (EndDateTime.Year == DateTime.Now.Year)
          {
            return EndDateTime.ToString("MMM d");
          }
          else
          {
            return EndDateTime.ToString("MMM d yyyy");
          }
        default:
          var sb = new StringBuilder();
          sb.Append(FormatDateRange(DateFormat.StartShort));
          if (StartDateTime.Hour > 0)
          {
            sb.Append(StartDateTime.ToString(" h:mm tt"));
          }
          if (EndDateTime > DateTime.MinValue)
          {
            sb.Append(" - ");
            if (EndDateTime.Date != StartDateTime.Date)
            {
              sb.Append(FormatDateRange(DateFormat.EndShort));
              if (EndDateTime.Hour > 0)
              {
                sb.Append(EndDateTime.ToString(" h:mm tt"));
              }
            }
          }
          return sb.ToString();
      }
    }

    public enum DateFormat
    {
      StartShort,
      Normal,
      EndShort
    }
  }
}