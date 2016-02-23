using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace frecevents.web.Models
{
  public class EventGroupModel
  {
    public string Name { get; set; }
    public List<EventModel> Events { get; set; }

    public EventGroupModel() { Events = new List<EventModel>();}
  }
}