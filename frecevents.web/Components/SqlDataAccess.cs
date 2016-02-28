using frecevents.web.Interfaces;
using frecevents.web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Components
{
    public class SqlDataAccess : IDataAccess
    {
        freceventsEntities ctx;
        private freceventsEntities Context { get { if (ctx == null) ctx = new freceventsEntities(); return ctx; } }

        public List<Models.EventModel> GetEvents()
        {
            var list = (from ei in Context.Events orderby ei.StartDateTime select new EventModel() { ID = ei.ID, StartDateTime = ei.StartDateTime, EndDateTime = ei.EndDateTime, Title = ei.Title }).ToList();
            return list;
        }

        public List<Models.EventModel> GetUpcomingEvents()
        {
            var list = (from ei in Context.Events 
                        where ei.StartDateTime >= DateTime.Now.Date || (ei.EndDateTime != DateTime.MinValue && ei.EndDateTime >= DateTime.Now.Date) 
                        orderby ei.StartDateTime 
                        select Models.EventModel.FromData(ei)).ToList();
            return list;
        }

        public List<Models.EventInfoModel> GetFeaturedEvents()
        {
            throw new NotImplementedException();
        }

        public Models.EventInfoModel GetEvent(string ID)
        {
            return EventInfoModel.FromData(Context.EventInfoes.Find(ID));
        }

        public void SaveEvent(Models.EventInfoModel Event)
        {
            var ev = GetEvent(Event.ID);
            if(ev==null)
            {
                try
                {
                    Context.EventInfoes.Add(Event.ToData());
                    Context.SaveChanges();
                }
                catch(Exception ex)
                {
                    ex.ToString();
                    throw;
                }
                return;
            }

            var original = Context.EventInfoes.Find(Event.ID);
            Context.Entry(original).CurrentValues.SetValues(Event);
            Context.SaveChanges();
        }

        public void DeleteEvent(string ID)
        {
            throw new NotImplementedException();
        }

        public void Register(Models.RegistrationModel reg)
        {
          var original = Context.Registrations.Find(reg.EventID, reg.RiderID);
          if (original != null)
          {
            Context.Entry(original).CurrentValues.SetValues(reg);
            Context.SaveChanges();
            return;
          }
          var newreg = Context.Registrations.Create();
          Context.Entry(newreg).CurrentValues.SetValues(reg);
          Context.SaveChanges();
        }

        public void Unregister(string EventID, int RiderID)
        {
          var original = Context.Registrations.Find(EventID, RiderID);
          if (original == null) return;
          Context.Registrations.Remove(original);
          Context.SaveChanges();
        }


        public RiderModel GetRider(int ID)
        {
          return RiderModel.FromData(Context.Riders.Find(ID));
        }

        public RiderModel FindRider(string Name, string Email)
        {
          RiderModel rider = null;
          if (!String.IsNullOrWhiteSpace(Name))
          {
            var riders = from r in Context.Riders where r.Name == Name select r;
            rider = RiderModel.FromData(riders.FirstOrDefault());
          }
          if (rider == null && !String.IsNullOrWhiteSpace(Email))
          {
            var ridersbyemail = from r in Context.Riders where r.Email == Email select r;
            rider = RiderModel.FromData(ridersbyemail.FirstOrDefault());
          }
          return rider;
        }

        public void SaveRider(RiderModel Rider)
        {
          if (Rider.ID > 0)
          {
            var original = Context.Riders.Find(Rider.ID);
            Context.Entry(original).CurrentValues.SetValues(Rider);
            Context.SaveChanges();
            return;
          }
          var foundrider = FindRider(Rider.Name, Rider.Email);
          if (foundrider != null)
          {
            Rider.ID = foundrider.ID;
            SaveRider(Rider);
          }
          var datarider = Rider.ToData();
          Context.Riders.Add(datarider);
          Context.SaveChanges();
          Rider.ID = datarider.ID;
        }
    }
}
