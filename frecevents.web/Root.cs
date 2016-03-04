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

    public static LoginInfo Login
    {
      get
      {
        var logindata = HttpContext.Current.Session["logindata"];
        LoginInfo login = null;

        if(logindata!=null)
        {
          login = LoginInfo.Parse(logindata.ToString());
          return login;
        }

        logindata = HttpContext.Current.Request.Cookies["logindata"];
        if(logindata!=null)
        {
          var value = ((HttpCookie)logindata).Value;
          HttpContext.Current.Session["logindata"] = value;
          login = LoginInfo.Parse(value);
        }

        return login;
      }
      set
      {
        SaveLogin(value.ToString());
      }
    }



    private static void SaveLogin(string logindata)
    {
      HttpContext.Current.Session["logindata"] = logindata;
      if(HttpContext.Current.Request.Cookies["logindata"]!=null)
      {
        HttpContext.Current.Response.Cookies.Remove("logindata");
        HttpContext.Current.Response.Cookies.Add(new HttpCookie("logindata", logindata) { Expires = System.DateTime.MaxValue });
      }
    }
  }
}
