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
        _login.Changed += (login, args) => SaveLogin(login.ToString());
        return _login;
      }
      set
      {
        if(_login !=null && (_login.UserType == value.UserType && _login.RiderID == value.RiderID))
        {
          return;
        }
        _login = value;
        _login.Changed += (login, args) => SaveLogin(login.ToString());

        SaveLogin(_login.ToString());
      }
    }

    private static void SaveLogin(string logindata)
    {
      HttpContext.Current.Session["logindata"] = logindata;
      if(HttpContext.Current.Request.Cookies["logindata"]!=null)
      {
        HttpContext.Current.Response.Cookies.Remove("logindata");
        HttpContext.Current.Response.Cookies.Add(new HttpCookie("logindata", _login.ToString()) { Expires = System.DateTime.MaxValue });
      }
    }
  }
}
