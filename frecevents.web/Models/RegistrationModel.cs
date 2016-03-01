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
      public int RegistrationRequest { get; set; }
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
        return new Registration() { eventID = this.EventID, RiderID = this.RiderID, Notes = this.Notes, TrailerSpace = this.TrailerSpace, RegistrationRequest = Convert.ToInt16(this.RegistrationRequest) };
      }
    }
}
