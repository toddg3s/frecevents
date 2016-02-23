using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Interfaces
{
    public interface IDataAccess
    {
        List<Models.EventModel> GetEvents();
        List<Models.EventModel> GetUpcomingEvents();
        List<Models.EventInfoModel> GetFeaturedEvents();
        Models.EventInfoModel GetEvent(string ID);
        void SaveEvent(Models.EventInfoModel Event);
        void DeleteEvent(string ID);

        void Register(Models.RegistrationModel reg);
        void Unregister(string EventID, int RiderID);
    }
}
