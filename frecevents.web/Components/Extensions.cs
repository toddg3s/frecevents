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

    public static bool Check2Bool(this string value)
    {
      if (String.IsNullOrEmpty(value))
        return false;

      var parts = value.Trim().Split(",;:/|\\^+*".ToCharArray());
      bool result = false;
      if (!Boolean.TryParse(parts[0], out result))
      {
        return false;
      }
      else
      {
        return result;
      }
    }
  }
}