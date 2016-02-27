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
      
    }
    
    public void Debug(string format, params object[] args)
    {
    }

    public void Info(string format, params object[] args)
    {
    }

    public void Warm(string format, params object[] args)
    {
    }

    public void Error(Exception ex, string format, params object[] args)
    {
    }
  }
}