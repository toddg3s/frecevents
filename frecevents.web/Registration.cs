//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace frecevents.web
{
    using System;
    using System.Collections.Generic;
    
    public partial class Registration
    {
        public string eventID { get; set; }
        public int RiderID { get; set; }
        public string Notes { get; set; }
        public int TrailerSpace { get; set; }
        public short RegistrationRequest { get; set; }
        public int LodingSpace { get; set; }
        public bool FoodVolunteer { get; set; }
    
        public virtual EventInfo EventInfo { get; set; }
        public virtual Rider Rider { get; set; }
    }
}
