using frecevents.web.Interfaces;
using frecevents.web.Components;

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
  }
}
