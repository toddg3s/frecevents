using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace keepalive
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(args[0]);
        request.Method = "GET";
        var response = (HttpWebResponse)request.GetResponse();
        Console.WriteLine(response.StatusCode.ToString() + ": " + response.ContentType + " (" + response.ContentLength + ")");
      }
      catch(WebException wex)
      {
        var message = String.Format("Exception while accessing {2}: {1} ({0})",wex.Status, wex.Message, args[0]);
        Console.WriteLine(message);
        EventLog.WriteEntry("keepalive", message);
      }
    }
  }
}
