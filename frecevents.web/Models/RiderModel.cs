using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace frecevents.web.Models
{
  public class RiderModel
  {
    public int ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public static RiderModel FromData(Rider rider)
    {
      if (rider == null) return null;
      return new RiderModel() {ID = rider.ID, Name = rider.Name, Email = rider.Email};
    }

    public Rider ToData()
    {
      return new Rider() { ID = this.ID, Name = this.Name, Email = this.Email };
    }
  }
}