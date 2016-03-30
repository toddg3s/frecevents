using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace frecevents.web.Models
{
  public class RiderModel : ModelBase
  {
    public int ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Trailerspace { get; set; }
    public int Lodgingspace { get; set; }
    public Int16 RegistrationRequest { get; set; }
    public bool FoodVolunteer { get; set; }

    public static RiderModel FromData(Rider rider)
    {
      if (rider == null) return null;
      return new RiderModel() {ID = rider.ID, Name = rider.Name, Email = rider.Email, Trailerspace = rider.Trailerspace, 
        Lodgingspace = rider.Lodgingspace, RegistrationRequest = rider.RegistrationRequest, FoodVolunteer = rider.FoodVolunteer };
    }

    public Rider ToData()
    {
      return new Rider()
      {
        ID = this.ID, Name = this.Name, Email = this.Email, Trailerspace = this.Trailerspace, 
        Lodgingspace = this.Lodgingspace, RegistrationRequest = this.RegistrationRequest, FoodVolunteer = this.FoodVolunteer
      };
    }
  }
}