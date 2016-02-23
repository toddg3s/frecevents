﻿using frecevents.web.Interfaces;
using frecevents.web.Models;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public void Unregister(string EventID, int RiderID)
        {
            throw new NotImplementedException();
        }
    }
}
