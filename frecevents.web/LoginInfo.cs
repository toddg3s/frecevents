using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web
{
  public class LoginInfo
  {
    public LoginType UserType { get; set; }
    public int RiderID { get; set; }

    public static LoginInfo Parse(string logindata)
    {
      if(String.IsNullOrWhiteSpace(logindata))
      {
        throw new ArgumentException("Empty logindata passed to Parse method", "logindata");
      }
      var parts = logindata.Split("|".ToCharArray());
      LoginType lt;
      int riderid = 0;

      if(!Enum.TryParse<LoginType>(parts[0], out lt))
      {
        throw new ArgumentException("Invalid logindata (" + logindata + ") passed to Parse method (type)", "logindata");
      }
      if(parts.Length > 1)
      {
        if(!Int32.TryParse(parts[1], out riderid))
        {
          throw new ArgumentException("Invalid logindata (" + logindata + ") passed to Parse method (id)", "logindata");
        }
      }
      return new LoginInfo() { UserType = lt, RiderID = riderid };
    }

    public override string ToString()
    {
      return Enum.GetName(typeof(LoginType), UserType) + "|" + RiderID.ToString();
    }
  }

  public enum LoginType
  {
    Member,
    Admin
  }
}
