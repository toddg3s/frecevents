using frecevents.web.Interfaces;
using frecevents.web.Components;
using System.Web;

namespace frecevents.web
{
  public class Root
  {
    private static IDataAccess _data;
    public static IDataAccess Data
    {
      get { return _data ?? (_data = new SqlDataAccess()); }
    }

    private static ICache _cache;
    public static ICache Cache
    {
      get { return _cache ?? (_cache = new ASPNetCache()); }
    }

    private static ILog _log;
    public static ILog Log
    {
      get { return _log ?? (_log = new Log4netLog()); }
    }

    private static LoginInfo _login;
    public static LoginInfo Login
    {
      get
      {
        if (_login != null) return _login;

        var logindata = HttpContext.Current.Session["logindata"];

        if(logindata!=null)
        {
          _login = LoginInfo.Parse(logindata.ToString());
          return _login;
        }

        logindata = HttpContext.Current.Request.Cookies["logindata"];
        if(logindata!=null)
        {
          var value = ((HttpCookie)logindata).Value;
          HttpContext.Current.Session["logindata"] = value;
          _login = LoginInfo.Parse(value);
        }

        return _login;
      }
      set
      {
        if(_login !=null && (_login.UserType == value.UserType && _login.RiderID == value.RiderID))
        {
          return;
        }
        _login = value;
        HttpContext.Current.Session["logindata"] = _login.ToString();
        if(HttpContext.Current.Request.Cookies["logindata"]!=null)
        {
          HttpContext.Current.Response.SetCookie(new HttpCookie("logindata", _login.ToString()));
        }
      }
    }
  }
}
