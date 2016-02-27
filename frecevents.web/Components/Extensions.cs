using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace frecevents.web
{
  public static class Extensions
  {
    public static bool IsEqual(this string value, string compareto)
    {
      var val = value.Trim();
      var compare = compareto.Trim();
      return val.Equals(compare, StringComparison.InvariantCultureIgnoreCase);
    }
  }
}