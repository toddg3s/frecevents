using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Interfaces
{
  public interface ILog
  {
    void Debug(string format, params object[] args);
    void Info(string format, params object[] args);
    void Warm(string format, params object[] args);
    void Error(Exception ex, string format, params object[] args);
  }
}
