using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using frecevents.web.Interfaces;

namespace frecevents.web.Components
{
  public class Log4netLog : ILog
  {
    public Log4netLog()
    {
      log4net.Config.XmlConfigurator.Configure();
    }

    private log4net.ILog _log;
    
    public void Debug(string format, params object[] args)
    {
      _log.DebugFormat(format, args);
    }

    public void Info(string format, params object[] args)
    {
      _log.InfoFormat(format, args);
    }

    public void Warm(string format, params object[] args)
    {
      _log.WarnFormat(format, args);
    }

    public void Error(Exception ex, string format, params object[] args)
    {
      _log.Error(String.Format(format, args), ex);
    }
  }
}