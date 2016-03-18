using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Models
{
    public class RegistrationModel
    {
      public string EventID { get; set; }
      public int RiderID { get; set; }
      public string Notes { get; set; }
      public int TrailerSpace { get; set; }
      public int LodgingSpace { get; set; }
      public Int16 RegistrationRequest { get; set; }
      public bool FoodVolunteer { get; set; }
      public bool Registered
      {
        get { return (RegistrationRequest > 0); }
      }

      public bool Request
      {
        get { return (RegistrationRequest == 1 || RegistrationRequest == 2); }
      }

      public Registration ToData()
      {
        return new Registration() { 
          eventID = this.EventID, 
          RiderID = this.RiderID, 
          Notes = this.Notes, 
          TrailerSpace = this.TrailerSpace, 
          LodingSpace = this.LodgingSpace,
          RegistrationRequest = Convert.ToInt16(this.RegistrationRequest),
          FoodVolunteer = this.FoodVolunteer
        };
      }

      public static RegistrationModel FromData(Registration reg)
      {
        return new RegistrationModel()
        {
          EventID =  reg.eventID, 
          RiderID =  reg.RiderID, 
          RegistrationRequest = reg.RegistrationRequest, 
          TrailerSpace = reg.TrailerSpace, 
          LodgingSpace = reg.LodingSpace,
          FoodVolunteer = reg.FoodVolunteer,
          Notes = reg.Notes
        };
      }
    }
}
