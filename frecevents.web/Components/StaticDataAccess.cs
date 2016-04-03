using frecevents.web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Components
{
  public class StaticDataAccess : IDataAccess
  {
    private List<Models.EventInfoModel> _events = new List<Models.EventInfoModel>() {
      new Models.EventInfoModel() { ID="test", StartDateTime=DateTime.Now.AddDays(1), EndDateTime = DateTime.Now.AddDays(2), EventSite="Gray Farms", 
      SiteURL="http://grayvalleyhome.com", Notes="Some notes", Lodging = true, Trailers = true, Description="Some test event", Title="Test Event"}
    };

    private List<Models.RiderModel> _riders = new List<Models.RiderModel>() {
      new Models.RiderModel() { ID = 1, Name = "Test Rider 1", Email="testrider1@freedomrun.events" },
      new Models.RiderModel() { ID = 2, Name = "Test Rider 2", Email="testrider2@freedomrun.events"}
    };

    public List<Models.EventModel> GetEvents()
    {
      return _events.Select(e => e as Models.EventModel).ToList();
    }

    public List<Models.EventModel> GetUpcomingEvents()
    {
      return _events.Select(e => e as Models.EventModel).ToList();
    }

    public List<Models.EventInfoModel> GetFeaturedEvents()
    {
      return _events;
    }

    public Models.EventInfoModel GetEvent(string ID)
    {
      return _events.Where(e => e.ID == ID).FirstOrDefault();
    }

    public void SaveEvent(Models.EventInfoModel Event)
    {
      var ev = GetEvent(Event.ID);
      if(ev!=null)
      {
        ev.Title = Event.Title;
        ev.StartDateTime = Event.StartDateTime;
        ev.EndDateTime = Event.EndDateTime;
        ev.Description = Event.Description;
        ev.EventSite = Event.EventSite;
        ev.EventType = Event.EventType;
        ev.SiteURL = Event.SiteURL;
        ev.MapUrl = Event.MapUrl;
        ev.Lodging = Event.Lodging;
        ev.Trailers = Event.Trailers;
      }
      else
      {
        _events.Add(Event);
      }
    }

    public void DeleteEvent(string ID)
    {
      var ev = GetEvent(ID);
      if(ev!=null)
      {
        _events.Remove(ev);
      }
    }

    public List<Models.RiderModel> GetRiders()
    {
      return _riders;
    }

    public Models.RiderModel GetRider(int ID)
    {
      return _riders.Where(r => r.ID == ID).FirstOrDefault();
    }

    public Models.RiderModel FindRider(string Name, string Email)
    {
      return _riders.Where(r =>
        (String.IsNullOrEmpty(Name) || r.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase)) &&
        (String.IsNullOrEmpty(Email) || r.Email.Equals(Email, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault();
    }

    public void SaveRider(Models.RiderModel Rider)
    {
      if(Rider.ID==0)
      {
        Rider.ID = _riders.Max(r => r.ID) + 1;
        _riders.Add(Rider);
      }
      else
      {
        var rider = GetRider(Rider.ID);
        if(rider!=null)
        {
          rider.Name = Rider.Name;
          rider.Email = Rider.Email;
        }
      }
    }

    public Models.RegistrationModel GetRegistration(string EventID, int RiderID)
    {
      var ev = GetEvent(EventID);
      if (ev == null) return null;
      return ev.Registrations.Where(reg => reg.RiderID == RiderID).FirstOrDefault();
    }

    public void Register(Models.RegistrationModel reg)
    {
      var ev = GetEvent(reg.EventID);
      if (ev == null) return;
      var curreg = ev.Registrations.Where(r => r.RiderID == reg.RiderID).FirstOrDefault();
      if(curreg==null)
      {
        ev.Registrations.Add(reg);
      }
      else
      {
        curreg.TrailerSpace = reg.TrailerSpace;
        curreg.LodgingSpace = reg.LodgingSpace;
        curreg.FoodVolunteer = reg.FoodVolunteer;
        curreg.Notes = reg.Notes;
      }
    }

    public void Unregister(string EventID, int RiderID)
    {
      var ev = GetEvent(EventID);
      if (ev == null) return;
      var reg = ev.Registrations.Where(r => r.RiderID == RiderID).FirstOrDefault();
      if (reg == null) return;
      ev.Registrations.Remove(reg);
    }
  }
}
