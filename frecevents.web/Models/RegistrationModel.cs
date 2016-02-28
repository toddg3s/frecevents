using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Models
{
    public class RegistrationModel : ModelBase
    {
      public string EventID { get; set; }
      public int RiderID { get; set; }
      public int TrailerSpace { get; set; }
    }
}
