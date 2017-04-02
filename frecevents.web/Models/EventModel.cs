using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace frecevents.web.Models
{
  public class EventModel : ModelBase
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
      var sb = new StringBuilder();
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
            return EndDateTime.ToString("MMM d, yyyy");
          }
        case DateFormat.StartLong:
          if (StartDateTime.Year == DateTime.Now.Year)
          {
            return StartDateTime.ToString("ddd, MMMM d");
          }
          else
          {
            return StartDateTime.ToString("ddd, MMMM d, yyyy");
          }
        case DateFormat.EndLong:
          if (EndDateTime == DateTime.MinValue)
          {
            return "";
          }
          if (EndDateTime.Year == DateTime.Now.Year)
          {
            return EndDateTime.ToString("ddd, MMMM d");
          }
          else
          {
            return EndDateTime.ToString("ddd, MMMM d, yyyy");
          }
        case DateFormat.StartFull:
          if (StartDateTime.Year == DateTime.Now.Year)
          {
            return StartDateTime.ToString("dddd, MMMM d");
          }
          else
          {
            return StartDateTime.ToString("dddd, MMMM d, yyyy");
          }
        case DateFormat.EndFull:
          if (EndDateTime == DateTime.MinValue)
          {
            return "";
          }
          if (EndDateTime.Year == DateTime.Now.Year)
          {
            return EndDateTime.ToString("dddd, MMMM d");
          }
          else
          {
            return EndDateTime.ToString("dddd, MMMM d, yyyy");
          }
        case DateFormat.List:
          if (StartDateTime == DateTime.MinValue) return "";
          sb.Append(FormatDateRange(DateFormat.StartLong));
          if (EndDateTime > DateTime.MinValue && EndDateTime.Date != StartDateTime.Date)
          {
            sb.Append(" - ");
            if(EndDateTime.Month != StartDateTime.Month)
            {
              sb.Append(EndDateTime.ToString("MMMM"));
              sb.Append(" ");
            }
            sb.Append(EndDateTime.Day);
          }
          return sb.ToString();
          break;
        default:
          sb.Append(FormatDateRange(DateFormat.StartLong));
          if (StartDateTime.Hour > 0)
          {
            sb.Append(StartDateTime.ToString(" h:mm tt"));
          }
          if (EndDateTime > DateTime.MinValue  && EndDateTime != StartDateTime)
          {
            sb.Append(" - ");
            if (EndDateTime.Date != StartDateTime.Date)
            {
              sb.Append(FormatDateRange(DateFormat.EndLong));
            }
            if (EndDateTime.Hour > 0)
            {
              sb.Append(EndDateTime.ToString(" h:mm tt"));
            }
          }
          return sb.ToString();
      }
    }

    public enum DateFormat
    {
      StartShort,
      Normal,
      EndShort,
      StartLong,
      EndLong,
      StartFull,
      EndFull,
      List
    }
  }
}